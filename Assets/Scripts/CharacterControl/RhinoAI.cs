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
    public new Camera camera;
    public TriggerCount meleeHitZone;
    public Canvas canvas;
    public float navMeshSampleRadius = 5f;
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
        foods = new List<Food>(GameObject.FindObjectsOfType<Food>());
    }

    void Start()
    {
        fsm = new FiniteStateMachine(new IdleState(this));
        fsm.Enter();

        agent.stoppingDistance += Vector3.Distance(
            transform.position,
            Vector3.ProjectOnPlane(
                camera.transform.position + camera.transform.forward * camera.nearClipPlane,
                transform.up
            )
        );
        agent.updatePosition = false;
        agent.isStopped = true;

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

    bool IsVisible(Vector3 target)
    {
        RaycastHit hit;
        return (
            !Physics.Linecast(
                camera.transform.position,
                target,
                out hit,
                LayerMask.NameToLayer("Enemy"),
                QueryTriggerInteraction.Ignore
            ) ||
            hit.collider.gameObject.Equals(gameObject)
        );
    }

    bool NavMeshSamplePosition(Vector3 target, out NavMeshHit hit)
    {
        return NavMesh.SamplePosition(target, out hit, navMeshSampleRadius + agent.stoppingDistance, GetNavMeshQueryFilter());
    }

    bool IsValidTarget(GameObject target, out NavMeshPath path)
    {
        path = new NavMeshPath();
        if (target == null)
        {
            return false;
        }

        Vector3 targetPosition = GetTargetPosition(target);
        if (!IsVisible(targetPosition))
        {
            return false;
        }

        NavMeshHit hit;
        if (!NavMeshSamplePosition(targetPosition, out hit))
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

    bool IsValidTarget(MonoBehaviour behaviour, out NavMeshPath path)
    {
        if (behaviour == null)
        {
            path = new NavMeshPath();
            return false;
        }
        return IsValidTarget(behaviour.gameObject, out path);
    }

    bool IsFacing(Vector3 target, out float angle)
    {
        Vector2 forward2 = new Vector2(transform.forward.x, transform.forward.z);
        Vector3 towards = target - transform.position;
        Vector2 towards2 = new Vector2(towards.x, towards.z);
        angle = Vector2.SignedAngle(towards2, forward2);
        return angle >= -facingAngleTolerance && angle <= facingAngleTolerance;
    }

    public bool IsAnimationPlaying(string tag)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag(tag);
    }

    static Vector3 GetTargetPosition(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            return rb.worldCenterOfMass;
        }
        return gameObject.transform.position;
    }

    private class IdleState : FSMState
    {
        private RhinoAI ai;
        private float wanderAt;

        public IdleState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public override void Enter()
        {
            wanderAt = Time.time + Random.Range(ai.minIdleTimeBeforeWander, ai.maxIdleTimeBeforeWander);
        }

        public override FSMState Execute()
        {
            if (ai.health.hp <= 0f)
            {
                return new DeadState(ai);
            }

            if (ai.IsValidTarget(ai.player, out _))
            {
                return new SeekState(ai, ai.player, ai.playerSeekWaitTime, new FightState(ai));
            }

            if (ai.health.hp < ai.health.maxHp)
            {
                foreach (Food food in ai.foods)
                {
                    if (ai.IsValidTarget(food, out _))
                    {
                        return new SeekState(ai, food.gameObject, 0f, new EatState(ai, food));
                    }
                }
            }

            if (Time.time > wanderAt)
            {
                return new WanderState(ai);
            }

            return this;
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
                return new IdleState(ai);
            }
            return this;
        }
    }

    private class SeekState : FSMState
    {
        private RhinoAI ai;
        private GameObject target;
        private float maxWaitTime;
        private FSMState nextState;
        private float waitingSince;

        public SeekState(RhinoAI ai, GameObject target, float maxWaitTime, FSMState nextState)
        {
            this.ai = ai;
            this.target = target;
            this.maxWaitTime = maxWaitTime;
            this.nextState = nextState;
            waitingSince = Time.time;
        }

        public override void Enter()
        {
            ai.agent.isStopped = false;
        }

        public override FSMState Execute()
        {
            if (!ai.IsValidTarget(target, out NavMeshPath path)
                || !ai.agent.SetPath(path))
            {
                if (Time.time - waitingSince >= maxWaitTime)
                {
                    return new IdleState(ai);
                }

                return this;
            }
            else
            {
                waitingSince = Time.time;
            }

            if (ai.agent.remainingDistance > ai.agent.stoppingDistance)
            {
                float speed = ai.agent.velocity.magnitude;
                ai.animator.SetFloat("speed", speed / ai.agent.speed);
                ai.animator.SetFloat("turn", 0f);

                return this;
            }

            if (ai.agent.remainingDistance < ai.minTargetRadius)
            {
                ai.animator.SetFloat("speed", -0.28f);
                ai.animator.SetFloat("turn", 0f);

                return this;
            }

            if (!ai.IsFacing(GetTargetPosition(target), out float angle))
            {
                float maxTurn = ai.turnInPlaceSpeed * Time.deltaTime;
                ai.transform.Rotate(Vector3.up, Mathf.Clamp(angle, -maxTurn, maxTurn));

                ai.animator.SetFloat("speed", 0f);
                ai.animator.SetFloat("turn", Mathf.Sign(angle));

                return this;
            }

            return nextState;
        }

        public override void Exit()
        {
            ai.agent.isStopped = true;
            ai.animator.SetFloat("speed", 0f);
            ai.animator.SetFloat("turn", 0f);
        }
    }

    private class WanderState : FSMState
    {
        private const int numAttempts = 30;
        private const float range = 20f;
        private RhinoAI ai;

        public WanderState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public override void Enter()
        {
            ai.agent.isStopped = false;

            for (int i = 0; i < numAttempts; i++)
            {
                Vector2 offset2 = Random.insideUnitCircle * range;
                Vector3 offset = new Vector3(offset2.x, 0f, offset2.y);
                Vector3 target = ai.transform.position + offset;
                if (NavMesh.SamplePosition(target, out NavMeshHit hit, 2f * ai.agent.height, ai.GetNavMeshQueryFilter())
                    && ai.agent.SetDestination(target))
                {
                    return;
                }
            }

            ai.agent.SetDestination(ai.transform.position);
        }

        public override FSMState Execute()
        {
            if (ai.agent.remainingDistance > ai.agent.stoppingDistance)
            {
                float speed = ai.agent.velocity.magnitude;
                ai.animator.SetFloat("speed", speed / ai.agent.speed);

                return this;
            }
            
            return new IdleState(ai);
        }

        public override void Exit()
        {
            ai.agent.isStopped = true;
            ai.animator.SetFloat("speed", 0f);
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
            if (!ai.IsValidTarget(ai.player, out NavMeshPath path)
                || !ai.agent.SetPath(path)
                || ai.agent.remainingDistance > ai.agent.stoppingDistance
                || !ai.IsFacing(GetTargetPosition(ai.player), out _))
            {
                return new SeekState(ai, ai.player, ai.playerSeekWaitTime, this);
            }

            return this;
        }

        public override void Exit()
        {
            ai.animator.SetBool("fighting", false);
            ai.weapon.SetCold();
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
            if (ai.IsAnimationPlaying("eat"))
            {
                animationQueued = false;
            }
            else if (!animationQueued)
            {
                if (food != null)
                {
                    Destroy(food.gameObject);
                    ai.health.ReceiveHealth();
                }
                return new IdleState(ai);
            }
            return this;
        }
    }
}
