using System;
using System.Transactions;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class RollingBallController : MonoBehaviour
{
    public enum RTBState
    {
        Patrolling,
        Startled,
        RunningAway,
        ReturningToPatrol
    }

    [Header("Movement")]
    [SerializeField] float rotationSpeed = 90f; // degrees per second
    [SerializeField] float jumpForce = 5f;

    // Physics-related movement members
    private float verticalVelocity;
    private float gravity = -9.8f;
    bool isJumping = false;
    bool isGrounded = true;
    float groundY;

    [Header("Patrol")]
    [SerializeField] Transform[] waypoints;
    private int destPoint = 0;

    [Header("Flee Behavior")]
    [SerializeField] float fleeRadius = 3f;
    [SerializeField] float fleeMaxSpeed = 10f;
    [SerializeField] float fleeTimer = 5f;
    private float fleeTimerCounter = 0f;

    [Header("Evade Behavior")]
    [SerializeField] float evadeWeight = 0.2f;

    private Vector3 tempVelocity;
    private Vector3 lastKnownPlayerLocation;

    float detectionRadius = 4f; // Should be refactored to be editable same as the FOV. Usecase for this is currently confusing.

    [Header("Frontal Detection range")]
    [Range(0, 360)] public float fovAngle = 180f;
    public float Fov;

    [Header("Debug")]
    [SerializeField] bool shouldBallBeStill;


    NavMeshAgent agent;
    Transform visualMesh;
    PlayerController player;
    RTBState currentState = RTBState.Patrolling;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        visualMesh = transform.Find("Mesh");
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (CanSeePlayer())
            Debug.Log("Can see player");


        float rollAmount = agent.velocity.magnitude * rotationSpeed * Time.deltaTime;
        visualMesh.Rotate(Vector3.right, rollAmount, Space.Self);

        switch (currentState)
        {
            case RTBState.Patrolling:
                Patrolling();
                break;
            case RTBState.Startled:
                JumpScare();
                break;
            case RTBState.RunningAway:
                if (fleeTimerCounter < fleeTimer)
                {
                    fleeTimerCounter += Time.deltaTime;
                    Flee();
                }
                else
                {
                    SetSafePatrolPoint();
                    fleeTimerCounter = 0f;
                }
                break;
            case RTBState.ReturningToPatrol:
                ReturnToPatrol();
                break;
        }

    }

    void JumpScare()
    {
        if (!isJumping && isGrounded)
            StartJump();

        if (isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime;

            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

            if (transform.position.y <= groundY)
            {
                Land();
                currentState = RTBState.RunningAway;
            }

        }
    }

    #region Patrol
    void Patrolling()
    {
        if (IsPlayerInRearHalfCircle() && !agent.isOnOffMeshLink)
        {
            Debug.Log("Was startled");
            currentState = RTBState.Startled;
            return;
        }

        GoToNextWaypoint();
    }

    void ReturnToPatrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            currentState = RTBState.Patrolling;
    }

    void SetSafePatrolPoint()
    {
        Debug.Log("SetSafePatrolPoint triggered");
        float dotProd = 0f;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 toWaypoint = waypoints[i].position - player.transform.position;
            dotProd = Vector3.Dot(player.transform.forward, toWaypoint.normalized);
            if (dotProd > 0)
            {
                agent.SetDestination(waypoints[i].position);
                destPoint = Array.IndexOf(waypoints, waypoints[i]);
                currentState = RTBState.ReturningToPatrol;
                return;
            }
        }

        if (dotProd < 0)
        {
            int rndIdx = UnityEngine.Random.Range(0, waypoints.Length);
            destPoint = rndIdx;
            agent.SetDestination(waypoints[destPoint].position);
            currentState = RTBState.ReturningToPatrol;
        }
    }

    private void GoToNextWaypoint()
    {
        if (agent.isOnOffMeshLink)
            return;

        agent.SetDestination(waypoints[destPoint].position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            destPoint = (destPoint + 1) % waypoints.Length;
        }
    }

    #endregion

    #region Detection
    bool IsPlayerInRearHalfCircle()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;

        if (toPlayer.magnitude > detectionRadius)
            return false;

        float dot = Vector3.Dot(transform.forward, toPlayer.normalized);
        return dot < 0f;
    }

    bool CanSeePlayer()
    {
        Collider[] targetsInFov = Physics.OverlapSphere(transform.position, Fov);

        foreach (Collider c in targetsInFov)
        {
            if (c.CompareTag("Player"))
            {
                float signedAngle = Vector3.Angle(transform.forward, c.transform.position - transform.position);

                if (Math.Abs(signedAngle) < fovAngle / 2)
                {
                    currentState = RTBState.RunningAway;
                    return true;
                }
            }
        }

        return false;
    }

    #endregion

    #region Steering Behavior


    private void Evade()
    {
        float distToMe = Vector3.Distance(transform.position, player.transform.position);
        float predictionTime = distToMe / agent.speed;

        Vector3 predictedTargetPosition = player.transform.position + player.playerVelocity * predictionTime;
        Vector3 desiredDirection = transform.position - predictedTargetPosition;
        Vector3 desiredVelocity = desiredDirection.normalized * fleeMaxSpeed;
        Vector3 steering = desiredVelocity - agent.velocity;


        agent.velocity += steering * evadeWeight;

    }

    private void Flee()
    {

        Vector3 toMe = transform.position - player.transform.position;
        Vector3 desiredVelocity = toMe.normalized * fleeMaxSpeed;
        Vector3 steering = desiredVelocity - agent.velocity;

        agent.velocity += steering;
    }

    #endregion

    #region Aerial Functions


    void StartJump()
    {
        isJumping = true;
        isGrounded = false;

        tempVelocity = agent.velocity;
        agent.velocity = Vector3.zero;

        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;

        groundY = transform.position.y;
        verticalVelocity = jumpForce;
    }

    void Land()
    {
        Vector3 pos = transform.position;
        pos.y = groundY;
        transform.position = pos;

        agent.velocity = tempVelocity;
        verticalVelocity = 0f;
        isJumping = false;
        isGrounded = true;

        agent.Warp(transform.position);
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        //Vector3 pos = transform.position;

        //// Radius
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(pos, detectionRadius);

        //Handles.color = Color.black;
        //Handles.DrawSolidDisc(transform.position, Vector3.up, 4f);

        //// Forward line
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(
        //    pos,
        //    pos + transform.forward * detectionRadius
        //);

        //// Rear direction
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(
        //    pos,
        //    pos - transform.forward * detectionRadius
        //);

        //// Side plane (visual divider)
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawLine(
        //    pos + transform.right * detectionRadius,
        //    pos - transform.right * detectionRadius
        //);
    }

}
