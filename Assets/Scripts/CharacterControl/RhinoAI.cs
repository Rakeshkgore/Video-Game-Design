using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GetHealth))]
[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(Invincibility))]
public class RhinoAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public new Camera camera;
    public TriggerCount meleeHitZone;
    public Canvas canvas;
    public float navMeshSampleRadiusPlayer = 5f;
    public float navMeshSampleRadiusFood = 2f;
    public float minTargetRadius = 2.333f;
    public float facingAngleTolerance = 10f;
    public float turnInPlaceSpeed = 45f;
    public float playerSeekWaitTime = 3f;
    public float minIdleTimeBeforeWander = 5f;
    public float maxIdleTimeBeforeWander = 10f;
    public float minTimeBeforeFireAttack = 10f;
    public float maxTimeBeforeFireAttack = 15f;
    public float invincibilityDuration = 3f;

    private NavMeshAgent agent;
    private Animator animator;
    private GameObject player;
    private GetBlessed playerBlessed;
    private Invincibility playerInvincibility;
    private GetHealth health;
    private Weapon weapon;
    private Invincibility invincibility;
    private List<Food> foods;
    private FiniteStateMachine fsm;
    private float nextFireAttack;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<GetHealth>();
        weapon = GetComponent<Weapon>();
        invincibility = GetComponent<Invincibility>();
        player = GameObject.FindWithTag("Player");
        playerBlessed = player.GetComponent<GetBlessed>();
        playerInvincibility = player.GetComponent<Invincibility>();
        foods = new List<Food>(GameObject.FindObjectsOfType<Food>());
    }

    void Start()
    {
        agent.stoppingDistance += Vector3.Distance(
            transform.position,
            Vector3.ProjectOnPlane(
                camera.transform.position + camera.transform.forward * camera.nearClipPlane,
                transform.up
            )
        );
        agent.updatePosition = false;
        agent.isStopped = true;

        fsm = new FiniteStateMachine(new PatrolState(this));
        fsm.Enter();

        SetNextFireAttackTime();
    }

    void Update()
    {
        fsm.Execute();

        if (Time.time >= nextFireAttack)
        {
            animator.SetTrigger("fire");
            nextFireAttack = float.PositiveInfinity;
        }
    }

    void OnAnimatorMove()
    {
        agent.nextPosition = animator.rootPosition;
        transform.position = agent.nextPosition;
    }

    IEnumerator FadeScene()
    {
        // Allow rhino death animation to play through
        yield return new WaitForSeconds(2f);

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        float duration = 0.4f;
        float counter = 0f;
        while (counter < duration) {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, counter / duration);
            yield return null;
        }
        SceneManager.LoadScene("Victory");
    }

    void OnWeaponHit(Weapon weapon)
    {
        if (health.hp > 0f && !invincibility.IsInvincible())
        {
            health.LoseHealth(weapon.Damage);

            if (health.hp <= 0f)
            {
                fsm.TransitionTo(new DeadState(this));
            }
            else
            {
                fsm.TransitionTo(new HitState(this));
                invincibility.SetInvincibleFor(invincibilityDuration);
            }
        }
    }

    void EmitFire()
    {
        SetNextFireAttackTime();
    }

    void SetNextFireAttackTime()
    {
        nextFireAttack = Time.time + Random.Range(minTimeBeforeFireAttack, maxTimeBeforeFireAttack);
    }

    bool NavMeshSamplePosition(Vector3 target, float radius, out NavMeshHit hit)
    {
        return NavMesh.SamplePosition(target, out hit, radius + agent.stoppingDistance, GetNavMeshQueryFilter());
    }

    bool IsValidTarget(GameObject target, float radius, out NavMeshPath path)
    {
        path = new NavMeshPath();
        if (target == null)
        {
            return false;
        }

        NavMeshHit hit;
        if (!NavMeshSamplePosition(target.transform.position, radius, out hit))
        {
            return false;
        }

        if (!NavMesh.CalculatePath(transform.position, hit.position, GetNavMeshQueryFilter(), path))
        {
            return false;
        }

        return path.status == NavMeshPathStatus.PathComplete;
    }

    NavMeshQueryFilter GetNavMeshQueryFilter()
    {
        NavMeshQueryFilter navMeshQueryFilter = new NavMeshQueryFilter();
        navMeshQueryFilter.agentTypeID = agent.agentTypeID;
        navMeshQueryFilter.areaMask = NavMesh.AllAreas;
        return navMeshQueryFilter;
    }

    bool IsValidTarget(MonoBehaviour behaviour, float radius, out NavMeshPath path)
    {
        if (behaviour == null)
        {
            path = new NavMeshPath();
            return false;
        }
        return IsValidTarget(behaviour.gameObject, radius, out path);
    }

    bool IsPlayerGoodTarget(out NavMeshPath path)
    {
        if (invincibility.IsInvincible() || playerInvincibility.IsInvincible())
        {
            path = new NavMeshPath();
            return false;
        }

        return IsValidTarget(player, navMeshSampleRadiusPlayer, out path);
    }

    bool IsFoodGoodTarget(Food food, out NavMeshPath path)
    {
        if (food != null && (invincibility.IsInvincible() || food.GetComponent<Collider>().attachedRigidbody.isKinematic))
        {
            path = new NavMeshPath();
            return false;
        }

        return IsValidTarget(food, navMeshSampleRadiusFood, out path);
    }

    bool IsFacing(Vector3 target, out float angle)
    {
        Vector2 forward2 = new Vector2(transform.forward.x, transform.forward.z);
        Vector3 towards = target - transform.position;
        Vector2 towards2 = new Vector2(towards.x, towards.z);
        angle = Vector2.SignedAngle(towards2, forward2);
        return angle >= -facingAngleTolerance && angle <= facingAngleTolerance;
    }

    bool HasReachedTarget()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    public bool IsAnimationPlaying(string tag)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag(tag);
    }

    private class PatrolState : FSMState
    {
        private RhinoAI ai;
        private int nextWaypointIndex;

        public PatrolState(RhinoAI ai)
        {
            this.ai = ai;
            nextWaypointIndex = GetNearestWaypoint();
        }
        public override void Enter()
        {
            ai.agent.isStopped = false;
            ai.agent.SetDestination(ai.patrolPoints[nextWaypointIndex].position);
        }

        public override FSMState Execute()
        {
            if (ai.IsPlayerGoodTarget(out _))
            {
                return new SeekPlayerState(ai);
            }

            if (ai.health.hp < ai.health.mhp)
            {
                foreach (Food food in ai.foods)
                {
                    if (ai.IsFoodGoodTarget(food, out _))
                    {
                        return new SeekFoodState(ai, food);
                    }
                }
            }

            if (ai.HasReachedTarget())
            {
                ++nextWaypointIndex;
                if (nextWaypointIndex >= ai.patrolPoints.Length)
                {
                    nextWaypointIndex = 0;
                }
                ai.agent.SetDestination(ai.patrolPoints[nextWaypointIndex].position);
            }

            float speed = ai.agent.velocity.magnitude;
            ai.animator.SetFloat("speed", speed / ai.agent.speed);
            ai.animator.SetFloat("turn", 0f);
            return this;
        }

        public override void Exit()
        {
            ai.agent.isStopped = true;
            ai.animator.SetFloat("speed", 0f);
            ai.animator.SetFloat("turn", 0f);
        }

        private int GetNearestWaypoint()
        {
            int nearestWaypointIndex = -1;
            float nearestWaypointDistance = float.PositiveInfinity;
            Vector3 currentPosition = ai.transform.position;

            for (int i = 0; i < ai.patrolPoints.Length; ++i)
            {
                Transform waypoint = ai.patrolPoints[i];
                float distance = Vector3.Distance(currentPosition, waypoint.position);
                if (distance < nearestWaypointDistance)
                {
                    nearestWaypointIndex = i;
                    nearestWaypointDistance = distance;
                }
            }

            return nearestWaypointIndex;
        }
    }

    private class DeadState : FSMState
    {
        private RhinoAI ai;

        public DeadState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public override void Enter()
        {
            ai.animator.SetBool("dead", true);
            ai.StartCoroutine(ai.FadeScene());
        }
    }

    private class HitState : FSMState
    {
        private RhinoAI ai;
        private bool animationQueued;

        public HitState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public override void Enter()
        {
            ai.animator.SetTrigger("hit");
            animationQueued = true;
        }

        public override FSMState Execute()
        {
            if (ai.IsAnimationPlaying("hit"))
            {
                animationQueued = false;
            }
            else if (!animationQueued)
            {
                return new PatrolState(ai);
            }
            return this;
        }
    }

    private class SeekPlayerState : FSMState
    {
        private RhinoAI ai;

        public SeekPlayerState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public override void Enter()
        {
            ai.agent.isStopped = false;
        }

        public override FSMState Execute()
        {
            if (!ai.IsPlayerGoodTarget(out NavMeshPath path)
                || !ai.agent.SetPath(path))
            {
                return new PatrolState(ai);
            }

            if (ai.meleeHitZone.IsTriggered)
            {
                return new FightState(ai);
            }

            if (!ai.HasReachedTarget())
            {
                float speed = ai.agent.velocity.magnitude;
                ai.animator.SetFloat("speed", speed / ai.agent.speed);
                ai.animator.SetFloat("turn", 0f);
                return this;
            }

            if (!ai.IsFacing(ai.player.transform.position, out float angle))
            {
                float maxTurn = ai.turnInPlaceSpeed * Time.deltaTime;
                ai.transform.Rotate(Vector3.up, Mathf.Clamp(angle, -maxTurn, maxTurn));

                ai.animator.SetFloat("speed", 0f);
                ai.animator.SetFloat("turn", Mathf.Sign(angle));
                return this;
            }

            if (ai.agent.remainingDistance < ai.minTargetRadius)
            {
                ai.animator.SetFloat("speed", -0.28f);
                ai.animator.SetFloat("turn", 0f);
                return this;
            }

            return new ShoutState(ai);
        }

        public override void Exit()
        {
            ai.agent.isStopped = true;
            ai.animator.SetFloat("speed", 0f);
            ai.animator.SetFloat("turn", 0f);
        }
    }

    private class SeekFoodState : FSMState
    {
        private RhinoAI ai;
        private Food food;

        public SeekFoodState(RhinoAI ai, Food food)
        {
            this.ai = ai;
            this.food = food;
        }

        public override void Enter()
        {
            ai.agent.isStopped = false;
        }

        public override FSMState Execute()
        {
            if (!ai.IsFoodGoodTarget(food, out NavMeshPath path)
                || !ai.agent.SetPath(path))
            {
                return new PatrolState(ai);
            }

            if (!ai.HasReachedTarget())
            {
                float speed = ai.agent.velocity.magnitude;
                ai.animator.SetFloat("speed", speed / ai.agent.speed);
                ai.animator.SetFloat("turn", 0f);
                return this;
            }

            if (!ai.IsFacing(food.transform.position, out float angle))
            {
                float maxTurn = ai.turnInPlaceSpeed * Time.deltaTime;
                ai.transform.Rotate(Vector3.up, Mathf.Clamp(angle, -maxTurn, maxTurn));

                ai.animator.SetFloat("speed", 0f);
                ai.animator.SetFloat("turn", Mathf.Sign(angle));
                return this;
            }

            if (ai.agent.remainingDistance < ai.minTargetRadius)
            {
                ai.animator.SetFloat("speed", -0.28f);
                ai.animator.SetFloat("turn", 0f);
                return this;
            }

            return new EatState(ai, food);
        }

        public override void Exit()
        {
            ai.agent.isStopped = true;
            ai.animator.SetFloat("speed", 0f);
            ai.animator.SetFloat("turn", 0f);
        }
    }

    private class FightState : FSMState
    {
        private RhinoAI ai;

        public FightState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public override void Enter()
        {
            ai.animator.SetBool("fighting", true);
        }

        public override FSMState Execute()
        {
            if (!ai.IsPlayerGoodTarget(out NavMeshPath path)
                || !ai.agent.SetPath(path))
            {
                return new PatrolState(ai);
            }

            if (ai.meleeHitZone.IsTriggered)
            {
                return this;
            }

            if (!ai.HasReachedTarget()
                || !ai.IsFacing(ai.player.transform.position, out float angle)
                || ai.agent.remainingDistance < ai.minTargetRadius)
            {
                return new SeekPlayerState(ai);
            }

            return new ShoutState(ai);
        }

        public override void Exit()
        {
            ai.animator.SetBool("fighting", false);
            ai.weapon.SetCold();
        }
    }

    private class ShoutState : FSMState
    {
        private RhinoAI ai;

        public ShoutState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public override void Enter()
        {
            ai.animator.SetBool("shouting", true);
        }

        public override FSMState Execute()
        {
            if (!ai.IsPlayerGoodTarget(out NavMeshPath path)
                || !ai.agent.SetPath(path))
            {
                return new PatrolState(ai);
            }

            if (ai.meleeHitZone.IsTriggered)
            {
                return new FightState(ai);
            }

            if (!ai.HasReachedTarget()
                || !ai.IsFacing(ai.player.transform.position, out float angle)
                || ai.agent.remainingDistance < ai.minTargetRadius)
            {
                return new SeekPlayerState(ai);
            }

            return this;
        }

        public override void Exit()
        {
            ai.animator.SetBool("shouting", false);
        }
    }

    private class EatState : FSMState
    {
        private RhinoAI ai;
        private bool animationQueued;
        public Food food { get; private set; }

        public EatState(RhinoAI ai, Food food)
        {
            this.ai = ai;
            this.food = food;
        }

        public override void Enter()
        {
            ai.animator.SetTrigger("eat");
            animationQueued = true;
        }

        public override FSMState Execute()
        {
            if (food == null)
            {
                return new PatrolState(ai);
            }

            if (ai.IsAnimationPlaying("eat"))
            {
                animationQueued = false;
            }
            else if (!animationQueued)
            {
                if (food != null)
                {
                    Destroy(food.transform.parent.gameObject);
                    ai.health.ReceiveHealth();
                }
                return new PatrolState(ai);
            }
            return this;
        }
    }
}
