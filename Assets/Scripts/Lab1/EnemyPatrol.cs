using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public enum GuardState
    {
        Patrolling,
        Chasing,
        Investigate,
        ReturningToPatrol
    }

    private NavMeshAgent agent;
    public GuardState currentState = GuardState.Patrolling;

    [Header("Target")]
    public Transform Player;

    

    [Header("Patrol Points")]
    public Transform[] patrolPoints;
    private int destPoint = 0;


    [Header("Aggro Transitions")]
    public float aggroRadius = 2f;
    public float fieldOfView = 5f;
    public float chaseRange = 10f;
    public float chaseTime = 5f;
    public float dropAggroTime = 5f;
    private float dropAggroCount = 0f;
    private Vector3 lastKnownPlayerPosition;

    [Header("Investigation")]
    public float InvestigationTime = 5f;
    private float investigationCounter = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.autoBraking = false; // Disables deceleration and acceleration when the agent reaches/recalculates its destination for smoother movement

        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        //agent.destination = patrolPoints[destPoint].position;
        agent.SetDestination(patrolPoints[destPoint].position);
        destPoint = (destPoint + 1) % patrolPoints.Length;
    }

    private void Update()
    {
        

        switch (currentState)
        {
            case GuardState.Patrolling:
                UpdatePatrol();
                break;
            case GuardState.Chasing:
                UpdateChase();
                break;
            case GuardState.Investigate:
                Investigate();
                break;
            case GuardState.ReturningToPatrol:
                UpdateReturning();
                break;
        }
    }
    void UpdatePatrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector3 agentDirection = transform.TransformDirection(Vector3.forward);
        Vector3 directionToPlayer = Vector3.Normalize(Player.transform.position - transform.position);

        float distance = Vector3.Distance(transform.position, Player.transform.position);

        if (distance <= aggroRadius)
        {
            currentState = GuardState.Chasing;
        }

        if (CanSeePlayer()) currentState = GuardState.Chasing;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(patrolPoints[destPoint].position);
            destPoint = (destPoint + 1) % patrolPoints.Length;
        }

    }
    void Investigate()
    {
        agent.SetDestination(lastKnownPlayerPosition);
        float distanceToLastKnown = Vector3.Distance(transform.position, lastKnownPlayerPosition);

        if (distanceToLastKnown < 0.5f)
        {
            investigationCounter += Time.deltaTime;

            if (investigationCounter >= InvestigationTime)
            {
                investigationCounter = 0f;
                currentState = GuardState.ReturningToPatrol;
            }
        }
    }

    void UpdateChase()
    {
        if (!CanSeePlayer())
        {
            if (dropAggroCount >= dropAggroTime)
            {
                currentState = GuardState.Investigate;
                dropAggroCount = 0f;
            }
            dropAggroCount += Time.deltaTime;
        }
        //agent.SetDestination(Player.transform.position);
        agent.SetDestination(Player.transform.position);

        float distance = Vector3.Distance(transform.position, Player.transform.position);

        if (distance >= chaseRange || chaseTime <= 0) currentState = GuardState.ReturningToPatrol;
    }
    void UpdateReturning()
    {
        agent.SetDestination(patrolPoints[destPoint].position);
        if (transform.position == patrolPoints[destPoint].position) currentState = GuardState.Patrolling;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw aggro radius
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, aggroRadius);

        // Draw FOV cone
        if (Application.isPlaying) // Only when playing
        {
            Vector3 forward = transform.forward;

            // Half-angle in radians
            float halfFOVRad = Mathf.Deg2Rad * 180 * 0.5f; // 180 deg

            // Calculate directions for the left and right edges of the FOV
            Vector3 leftEdge = Quaternion.Euler(0, -180 * 0.5f, 0) * forward;
            Vector3 rightEdge = Quaternion.Euler(0, 180 * 0.5f, 0) * forward;

            // Draw lines for the edges
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + leftEdge * chaseRange);
            Gizmos.DrawLine(transform.position, transform.position + rightEdge * chaseRange);

            // Draw line to player
            if (Player != null)
            {
                Gizmos.color = CanSeePlayer() ? Color.red : Color.green;
                Gizmos.DrawLine(transform.position, Player.position);
            }
        }
    }

        bool CanSeePlayer()
    {
        Vector3 toPlayer = Player.position - transform.position;

        // The dot prod will server as our field of view. The Cos() calc will serve to check if the result is within the fov but with a half angle because
        // we check from center view to each side (left, right, ie, 90 deg to the left and 90 deg to the right, based on center view).
        // If we reduce the number 180 to, say, 120 deg, then our fov would narrow and the player would have to be me "more" within our direct field of view.

        float dotProd = Vector3.Dot(transform.forward, toPlayer.normalized);
        float aggroVision = Mathf.Cos(180 * 0.5f * Mathf.Deg2Rad);
        if (dotProd < aggroVision) return false;

        if (toPlayer.magnitude > chaseRange) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, toPlayer.normalized, out hit, chaseRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                lastKnownPlayerPosition = hit.transform.position; 
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

}

