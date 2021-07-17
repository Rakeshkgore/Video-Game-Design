using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GetHealth))]
public class RhinoAI : MonoBehaviour
{
    public new Camera camera;
    public Canvas canvas;
    private NavMeshAgent agent;
    private Animator animator;
    private GameObject player;
    private GetHealth health;
    private List<Food> foods;
    private State state;
    private bool fadeSceneStarted;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<GetHealth>();
        player = GameObject.FindWithTag("Player");
        foods = new List<Food>(GameObject.FindObjectsOfType<Food>());
        fadeSceneStarted = false;
    }

    void Start()
    {
        state = new IdleState(this);
        agent.stoppingDistance += Vector3.Distance(
            transform.position,
            Vector3.ProjectOnPlane(
                camera.transform.position + camera.transform.forward * camera.nearClipPlane,
                transform.up
            )
        );
    }

    void Update()
    {
        state = state.Execute();
        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);
        animator.SetBool("fighting", state is FightState);
        animator.SetBool("eating", state is EatState);
        animator.SetBool("dead", state is DeadState);

        if (state is DeadState && !fadeSceneStarted)
        {
            StartCoroutine(FadeScene());
            fadeSceneStarted = true;
        }
    }

    void OnAnimatorMove()
    {
        agent.nextPosition = animator.rootPosition;
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

    bool IsVisible(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        Vector3 worldPoint = gameObject.transform.position;
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            worldPoint = rb.worldCenterOfMass;
        }

        // FIXME
        if (gameObject.CompareTag("Player") && gameObject.transform.position.x < -34.33073)
        {
            return false;
        }

        RaycastHit hit;
        return (
            !Physics.Linecast(
                camera.transform.position,
                worldPoint,
                out hit,
                LayerMask.NameToLayer("Enemy"),
                QueryTriggerInteraction.Ignore
            ) ||
            hit.collider.gameObject.Equals(gameObject)
        );
    }

    bool IsVisible(MonoBehaviour behaviour)
    {
        if (behaviour == null)
        {
            return false;
        }
        return IsVisible(behaviour.gameObject);
    }

    void OnWeaponHit(Weapon weapon)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("hit"))
        {
            health.LoseHealth(weapon.Damage);
            if (health.hp <= 0f)
            {
                state = new DeadState(this);
            }
            else
            {
                animator.SetTrigger("hit");
                state = new HitState(this);
            }
        }
    }

    private interface State
    {
        State Execute();
    }

    private class IdleState : State
    {
        private RhinoAI ai;

        public IdleState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public State Execute()
        {
            if (ai.health.hp <= 0f)
            {
                return new DeadState(ai);
            }

            if (ai.IsVisible(ai.player))
            {
                return new SeekState(ai, ai.player);
            }

            foreach (Food food in ai.foods)
            {
                if (ai.IsVisible(food))
                {
                    return new SeekState(ai, food);
                }
            }

            ai.agent.isStopped = true;
            return this;
        }
    }

    private class DeadState : State
    {
        private RhinoAI ai;

        public DeadState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public State Execute()
        {
            ai.agent.isStopped = true;
            return this;
        }
    }

    private class HitState : State
    {
        private RhinoAI ai;

        public HitState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public State Execute()
        {
            ai.agent.isStopped = true;

            AnimatorStateInfo animatorState = ai.animator.GetCurrentAnimatorStateInfo(0);

            if (animatorState.IsTag("hit") && animatorState.normalizedTime < 1f)
            {
                return this;
            }

            return new IdleState(ai);
        }
    }

    private class SeekState : State
    {
        private RhinoAI ai;
        private GameObject target;

        public SeekState(RhinoAI ai, GameObject target)
        {
            this.ai = ai;
            this.target = target;
        }

        public SeekState(RhinoAI ai, MonoBehaviour target) : this(ai, target.gameObject) {}

        public State Execute()
        {
            if (ai.health.hp <= 0f)
            {
                return new DeadState(ai);
            }

            if (!ai.IsVisible(target))
            {
                return new IdleState(ai);
            }

            ai.agent.SetDestination(target.transform.position);
            ai.agent.isStopped = false;

            if (!ai.agent.pathPending
                && ai.agent.remainingDistance <= ai.agent.stoppingDistance
                && ai.agent.velocity.sqrMagnitude == 0f
                && ai.agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                Vector2 forward2 = new Vector2(ai.transform.forward.x, ai.transform.forward.z);
                Vector3 towards = target.transform.position - ai.transform.position;
                Vector2 towards2 = new Vector2(towards.x, towards.z);
                float angle = Vector2.SignedAngle(towards2, forward2);

                if (Mathf.Abs(angle) > 15f)
                {
                    ai.animator.SetFloat("turn", Mathf.Sign(angle));
                    ai.transform.Rotate(Vector3.up, Mathf.Clamp(angle, -45f * Time.deltaTime, 45f * Time.deltaTime));
                }
                else
                {
                    ai.animator.SetFloat("turn", 0f);
                    if (target.CompareTag("Player"))
                    {
                        return new FightState(ai);
                    }

                    Food food;
                    if (target.TryGetComponent<Food>(out food))
                    {
                        return new EatState(ai, food);
                    }

                    return new IdleState(ai);
                }
            }

            return this;
        }
    }

    private class FightState : State
    {
        private RhinoAI ai;

        public FightState(RhinoAI ai)
        {
            this.ai = ai;
        }

        public State Execute()
        {
            if (ai.health.hp <= 0f)
            {
                return new DeadState(ai);
            }

            if (!ai.IsVisible(ai.player))
            {
                return new IdleState(ai);
            }

            ai.agent.SetDestination(ai.player.transform.position);
            ai.agent.isStopped = true;

            if (!ai.agent.pathPending && (
                    ai.agent.remainingDistance > ai.agent.stoppingDistance
                    || ai.agent.pathStatus != NavMeshPathStatus.PathComplete
                ))
            {
                return new SeekState(ai, ai.player);
            }

            Vector2 forward2 = new Vector2(ai.transform.forward.x, ai.transform.forward.z);
            Vector3 towards = ai.player.transform.position - ai.transform.position;
            Vector2 towards2 = new Vector2(towards.x, towards.z);
            float angle = Vector2.SignedAngle(towards2, forward2);

            if (Mathf.Abs(angle) > 15f)
            {
                return new SeekState(ai, ai.player);
            }

            return this;
        }
    }

    private class EatState : State
    {
        private RhinoAI ai;
        private Food food;

        public EatState(RhinoAI ai, Food food)
        {
            this.ai = ai;
            this.food = food;
        }

        public State Execute()
        {
            if (ai.health.hp <= 0f)
            {
                return new DeadState(ai);
            }

            AnimatorStateInfo animatorState = ai.animator.GetCurrentAnimatorStateInfo(0);

            if (animatorState.IsTag("eat") && animatorState.normalizedTime >= 1)
            {
                if (food != null)
                {
                    Destroy(food.gameObject);
                    ai.foods.Remove(food);
                }
                return new IdleState(ai);
            }

            return this;
        }
    }
}
