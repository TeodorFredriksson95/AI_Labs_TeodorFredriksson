using System;
using System.Transactions;
using Unity.Behavior;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public enum TRBState
{
    Patrolling,
    Startled,
    RunningAway,
    ReturningToPatrol,
    Chilling
}

public class RollingBallController : MonoBehaviour
{

    [SerializeField] private BehaviorGraphAgent behaviorAgent;
    [SerializeField] private RendezvousCoordinator coordinator;
    private BlackboardVariable<TrbWannaChillEvent> TRBWannaChillChannel;

    // Helper agent state reference
    public States helperAgentState;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float jumpForce = 5f;

    // Physics-related movement members
    private float verticalVelocity;
    private float gravity = -9.8f;
    private bool isJumping = false;
    private bool isGrounded = true;
    private float groundY;

    [Header("Patrol")]
    [SerializeField] Transform[] waypoints;
    private int destPoint = 0;
    [SerializeField] private float readyToChillTime = 5f;
    private float chillTimeCounter;

    [Header("Common Safepoint")]
    [SerializeField] private Transform safetyPoint;
    [SerializeField] private float breakTimeDuration;
    private float breakTimeCounter;
    [SerializeField] private float breakTimeCooldown = 10f;
    private float breakTimeCooldownCounter;
    private bool isBreakOnCooldown => breakTimeCooldownCounter < breakTimeCooldown;
    private bool isOnBreak;

    [Header("Flee Behavior")]
    [SerializeField] private float fleeRadius = 3f;
    [SerializeField] private float fleeMaxSpeed = 10f;
    [SerializeField] private float fleeTimer = 5f;
    private float fleeTimerCounter = 0f;

    [Header("Evade Behavior")]
    [SerializeField] float evadeWeight = 0.2f;

    private Vector3 tempVelocity;
    private Vector3 lastKnownPlayerLocation;

    private float detectionRadius = 4f; // Should be refactored to be editable same as the FOV. Usecase for this is currently confusing.

    [Header("Frontal Detection range")]
    [Range(0, 360)] public float fovAngle = 180f;
    public float Fov;

    [Header("Debug")]
    [SerializeField] private bool shouldBallBeStill;
    [SerializeField] private TRBState currentState = TRBState.Patrolling;


    private NavMeshAgent agent;
    private Transform visualMesh;
    private PlayerController player;


    // Jump related stuff
    private float jumpTimer = 3f;
    private float jumpTimerCounter;

    public TRBState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        visualMesh = transform.Find("Mesh");
        player = FindFirstObjectByType<PlayerController>();
        behaviorAgent.GetVariable("TRBChillEvent", out TRBWannaChillChannel);

        breakTimeCooldownCounter = breakTimeCooldown; // If we dont initialize the counter to be = to the cooldown,
                                                      // the ability is considered to be ON cooldown at the start of the game.
                                                      // It causes weird timings with the Helper NPC otherwise.

    }

    void Update()
    {
        if (CanSeePlayer())
            Debug.Log("Can see player");

        chillTimeCounter += Time.deltaTime;

        if (isOnBreak)
            breakTimeCounter += Time.deltaTime;

        if (isBreakOnCooldown)
            breakTimeCooldownCounter += Time.deltaTime;

        DoesTRBWannaChill();

        //if (CanTRBTransitionToChill())
        //{
        //    coordinator.IsTRBReady(isReady: true);
        //}

        float rollAmount = agent.velocity.magnitude * rotationSpeed * Time.deltaTime;
        visualMesh.Rotate(Vector3.right, rollAmount, Space.Self);

        switch (currentState)
        {
            case TRBState.Patrolling:
                Patrolling();
                break;
            case TRBState.Startled:
                JumpScare();
                break;
            case TRBState.RunningAway:
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
            case TRBState.ReturningToPatrol:
                ReturnToPatrol();
                break;
            case TRBState.Chilling:
                MoveToSafePoint();
                IsBreakOver();
                break;
        }

    }




    #region Coordinate Rendezvous point

    private void DoesTRBWannaChill()
    {
        if (chillTimeCounter >= readyToChillTime && currentState == TRBState.Patrolling)
            TRBWannaChillChannel.Value.SendEventMessage();

    }

    public void GoOnBreak()
    {
        isOnBreak = true;
        currentState = TRBState.Chilling;
    }

    public bool CanTRBTransitionToChill()
    {
        //if (helperAgentState != States.GoToChillPoint)
        //    return false;

        if (isBreakOnCooldown)
            return false;

        if (currentState != TRBState.Patrolling)
        {
            return false;
        }

        if (chillTimeCounter < readyToChillTime)
        {
            return false;
        }

        chillTimeCounter = 0f;
        return true;
    }

    public void MoveToSafePoint()
    {
        Debug.Log("TRB goes on break");

        jumpTimerCounter += Time.deltaTime;

        agent.stoppingDistance = 10f;

        agent.SetDestination(safetyPoint.position);

        if (!agent.pathPending && agent.remainingDistance < 5f && jumpTimerCounter >= jumpTimer)
        {
            JumpDuringBreak();
        }

    }

    private void JumpDuringBreak()
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
                jumpTimerCounter = 0f;
            }

        }
    }

    private bool IsBreakOver()
    {
        if (breakTimeCounter >= breakTimeDuration)
        {
            BreakIsOver();
            return true;
        }

        return false;
    }

    public void BreakIsOver()
    {
        breakTimeCooldownCounter = 0f;
        isOnBreak = false;
        breakTimeCounter = 0f;
        chillTimeCounter = 0f;
        agent.stoppingDistance = 2f;
        SetSafePatrolPoint();
    }

    private void DoSomethingDuringBreak()
    {
        // Do something during break?
    }


    #endregion
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
                currentState = TRBState.RunningAway;
            }

        }
    }

    #region Patrol
    void Patrolling()
    {
        if (IsPlayerInRearHalfCircle() && !agent.isOnOffMeshLink)
        {
            Debug.Log("Was startled");
            currentState = TRBState.Startled;
            return;
        }

        GoToNextWaypoint();
    }

    void ReturnToPatrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            currentState = TRBState.Patrolling;
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
                currentState = TRBState.ReturningToPatrol;
                return;
            }
        }

        if (dotProd < 0)
        {
            int rndIdx = UnityEngine.Random.Range(0, waypoints.Length);
            destPoint = rndIdx;
            agent.SetDestination(waypoints[destPoint].position);
            currentState = TRBState.ReturningToPatrol;
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

                if (Mathf.Abs(signedAngle) < fovAngle / 2)
                {
                    currentState = TRBState.RunningAway;
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
