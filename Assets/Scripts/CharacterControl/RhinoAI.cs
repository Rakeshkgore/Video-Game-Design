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
    private float winAt = float.NaN;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<GetHealth>();
        player = GameObject.FindWithTag("Player");
        foods = new List<Food>(GameObject.FindObjectsOfType<Food>());
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

        if (state is DeadState)
        {
            if (float.IsNaN(winAt))
            {
                winAt = Time.time + 2f;
            }
            if (Time.time >= winAt)
            {
                StartCoroutine(FadeScene());
            }
        }
    }

    IEnumerator FadeScene()
    {
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
        Vector3 viewportPoint = camera.WorldToViewportPoint(worldPoint);
        RaycastHit hit;

        return (
            viewportPoint.x >= 0 &&
            viewportPoint.x <= 1 &&
            viewportPoint.y >= 0 &&
            viewportPoint.y <= 1 &&
            viewportPoint.z >= camera.nearClipPlane &&
            viewportPoint.z <= camera.farClipPlane &&
            (
                !Physics.Linecast(
                    transform.position,
                    worldPoint,
                    out hit,
                    Physics.AllLayers,
                    QueryTriggerInteraction.Ignore
                ) ||
                hit.collider.gameObject.Equals(gameObject)
            )
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

            if (!ai.agent.pathPending && ai.agent.remainingDistance <= ai.agent.stoppingDistance)
            {
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

            if (!ai.agent.pathPending && ai.agent.remainingDistance > ai.agent.stoppingDistance)
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
