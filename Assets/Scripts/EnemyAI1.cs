using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI1 : MonoBehaviour
{
    private enum State { Patrol, Chase, Talk }  
    [SerializeField] private Transform[] waypoints;

    [Header("OffMeshLink / Jump")]
    [SerializeField] private string jumpTriggerName = "Jump";
    [SerializeField] private string landTriggerName = "";      
    [SerializeField] private float jumpDuration = 0.6f;
    [SerializeField] private float jumpHeight   = 1.2f;
    [SerializeField] private bool useParabolaMovement = true;  
    
    [SerializeField] private AudioSource sfxSource;
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
    private int HashJump;
    private int HashLand;

    private bool isJumping;

    void Start()
    {
        HashSpeed = Animator.StringToHash("Speed");
        HashJump  = Animator.StringToHash(jumpTriggerName);
        HashLand  = string.IsNullOrEmpty(landTriggerName) ? 0 : Animator.StringToHash(landTriggerName);

        agent.autoTraverseOffMeshLink = false;

        if (waypoints != null && waypoints.Length > 0)
            agent.SetDestination(waypoints[wpIndex].position);
    }

    void Update()
    {
        if (anim) anim.SetFloat(HashSpeed, agent.velocity.magnitude);

        if (!isJumping && agent.isOnOffMeshLink)
            StartCoroutine(TraverseOffMeshLinkWithJump());

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
            
            if (sfxSource != null && talkSfx != null) 
                sfxSource.PlayOneShot(talkSfx);
            
            lastTalkTime = Time.time;
        }

        float d = Vector3.Distance(transform.position, objective.position);
        
        if (d > talkRange) 
        { 
            currentState = State.Chase; 
            anim.SetBool("isTalking", false); 
        }
        // Si pierde de vista al jugador, volver a patrullar
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
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !agent.isOnOffMeshLink)
        {
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
        Vector3 rightBoundary = Quaternion.Euler(0,  viewAngle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position + Vector3.up, leftBoundary  * viewRadius);
        Gizmos.DrawRay(transform.position + Vector3.up, rightBoundary * viewRadius);
    }
    
    private IEnumerator TraverseOffMeshLinkWithJump()
    {
        isJumping = true;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 start = data.startPos;
        Vector3 end   = data.endPos;

        FaceHorizontally(end, 18f);

        if (HashJump != 0) anim.SetTrigger(HashJump);

        if (!useParabolaMovement)
        {
            agent.updatePosition = false;

            float timer = 0f;
            while (timer < jumpDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            agent.updatePosition = true;
        }
        else
        {
            agent.updatePosition = false;

            float t = 0f;
            Vector3 origin = start;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(0.001f, jumpDuration);
                Vector3 pos = Parabola(origin, end, jumpHeight, Mathf.Clamp01(t));
                transform.position = pos;

                Vector3 dir = (end - transform.position); dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 12f);

                yield return null;
            }

            agent.updatePosition = true;
        }

        if (HashLand != 0) anim.SetTrigger(HashLand);

        agent.CompleteOffMeshLink();
        isJumping = false;
    }

    private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 m = Vector3.Lerp(start, end, t);
        float y = -4f * height * (t - 0.5f) * (t - 0.5f) + height;
        m.y += y;
        return m;
    }

    private void FaceHorizontally(Vector3 target, float turnSpeed)
    {
        Vector3 dir = target - transform.position; dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * turnSpeed);
    }
    
}