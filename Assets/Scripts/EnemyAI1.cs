using UnityEngine;
using UnityEngine.AI;

public class EnemyAI1 : MonoBehaviour
{
    private enum State { Patrol, Chase, Talk }
    [SerializeField] private Transform[] waypoints;

    [Header("Audio")]
    [SerializeField] private AudioSource enemyAudioSource;
    [SerializeField] private AudioClip talkSfx;

    private int wpIndex = 0;
    private State currentState = State.Patrol;

    private Animator anim => GetComponentInChildren<Animator>();
    private NavMeshAgent agent => GetComponent<NavMeshAgent>();

    // Chase
    [SerializeField] private float viewRadius = 10.0f;
    [SerializeField] private float viewAngle = 90.0f;
    [SerializeField] private Transform objective;
    private float loseSightTimer = 0.0f;
    [SerializeField] private float loseSightTime = 3.0f;

    // Talk 
    [SerializeField] private float talkRange = 2.0f;
    [SerializeField] private float talkCooldown = 3.0f;
    private float lastTalkTime = 0.0f;

    // Animator hashes
    private int HashSpeed;

    void Start()
    {
        HashSpeed = Animator.StringToHash("Speed");

        if (waypoints != null && waypoints.Length > 0)
            agent.SetDestination(waypoints[wpIndex].position);
    }

    void Update()
    {
        if (anim) anim.SetFloat(HashSpeed, agent.velocity.magnitude);

        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase:  Chase();  break;
            case State.Talk:   Talk();   break;
        }
    }

    private void Talk()
    {
        if (!objective) return;

        transform.LookAt(objective);
        agent.SetDestination(transform.position);

        if (Time.time > lastTalkTime + talkCooldown)
        {
            anim.SetBool("isTalking", true);

            if (talkSfx != null && enemyAudioSource != null)
                enemyAudioSource.PlayOneShot(talkSfx);

            lastTalkTime = Time.time;
        }

        float d = Vector3.Distance(transform.position, objective.position);

        if (d > talkRange)
        {
            currentState = State.Chase;
            anim.SetBool("isTalking", false);
        }
        else if (!LookForObjective())
        {
            currentState = State.Patrol;
            anim.SetBool("isTalking", false);
        }
    }

    private void Chase()
    {
        if (!objective) { currentState = State.Patrol; return; }

        agent.speed = 5.0f;
        agent.SetDestination(objective.position);

        float d = Vector3.Distance(transform.position, objective.position);

        if (d <= talkRange)
        {
            currentState = State.Talk;
            anim.SetTrigger("isTalking");
        }
        else if (d > viewRadius)
        {
            loseSightTimer += Time.deltaTime;
            if (loseSightTimer >= loseSightTime)
            {
                currentState = State.Patrol;
                loseSightTimer = 0.0f;
            }
        }
        else loseSightTimer = 0.0f;
    }

    private void Patrol()
    {
        agent.speed = 2.5f;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (waypoints == null || waypoints.Length == 0) return;
            wpIndex = (wpIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[wpIndex].position);
        }

        if (LookForObjective()) currentState = State.Chase;
    }

    private bool LookForObjective()
    {
        if (!objective) return false;
        Vector3 to = (objective.position - transform.position);
        if (to.magnitude > viewRadius) return false;

        float angle = Vector3.Angle(transform.forward, to.normalized);
        if (angle > viewAngle * 0.5f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, to.normalized, out RaycastHit hit, viewRadius))
            return hit.transform == objective;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = LookForObjective() ? Color.green : Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position + Vector3.up, leftBoundary * viewRadius);
        Gizmos.DrawRay(transform.position + Vector3.up, rightBoundary * viewRadius);
    }
}
