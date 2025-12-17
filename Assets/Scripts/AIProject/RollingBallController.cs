using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class RollingBallController : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 90f; // degrees per second
    [SerializeField] Transform[] waypoints;
    private int destPoint = 0;

    NavMeshAgent agent;

    Transform visualMesh;
    PlayerController player;

    [SerializeField] float fleeRadius = 3f;
    [SerializeField] float fleeMaxSpeed = 10f;
    [SerializeField] float fleeTimer = 5f;
    [SerializeField] float evadeWeight = 0.2f;
    private float fleeTimerCounter = 0f;
    private bool isFleeing = false;

    [SerializeField] private Vector3 currentVelocity;

    Transform farthestWaypoint;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        visualMesh = transform.Find("Mesh");
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        currentVelocity = agent.velocity;

        float distanceToPlayer = (transform.position - player.transform.position).magnitude;

        float rollAmount = agent.velocity.magnitude * rotationSpeed * Time.deltaTime;
        visualMesh.Rotate(Vector3.right, rollAmount, Space.Self);

        if (!isFleeing)
            GoToNextWaypoint(true);

        if (distanceToPlayer < fleeRadius)
        {
            isFleeing = true;

            if (isFleeing && fleeTimerCounter < fleeTimer)
            {
                fleeTimerCounter += Time.deltaTime;
                Flee();
            }
        }
        if (fleeTimerCounter >= fleeTimer)
        {
            isFleeing = false;
            fleeTimerCounter = 0f;
        }
    }

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
        Debug.Log("Flee was triggered");

        Vector3 toMe = transform.position - player.transform.position;
        Vector3 desiredVelocity = toMe.normalized * fleeMaxSpeed;
        Vector3 steering = desiredVelocity - agent.velocity;
        //steering = Vector3.ClampMagnitude(steering, fleeMaxSpeed);

        float dotProd;

        float distWaypointToPlayer = float.MinValue;

        for (int i = 0; i < waypoints.Length; i++)
        {
            dotProd = Vector3.Dot(transform.position, waypoints[i].position);
            if (dotProd > 0)
            {
                agent.SetDestination(waypoints[i].position);
                break;
            }

            //float waypointDist = Vector3.Distance(waypoints[i].position, player.transform.position);

            //if (waypointDist > distWaypointToPlayer)
            //{
            //    distWaypointToPlayer = waypointDist;
            //    agent.SetDestination(waypoints[i].position);
            //}


        }

        agent.velocity += steering;
    }

    private void GoToNextWaypoint(bool isWaypointRandom)
    {
        if (agent.isOnOffMeshLink)
            return;

        if (isWaypointRandom)
        {
            int randomIndex = Random.Range(0, waypoints.Length);
            agent.SetDestination(waypoints[randomIndex].position);
        }
        else agent.SetDestination(waypoints[destPoint].position);


        float distanceToPoint = (transform.position - waypoints[destPoint].position).magnitude;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            destPoint = (destPoint + 1) % waypoints.Length;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.black;
        Handles.DrawSolidDisc(transform.position, Vector3.up, 3f);
    }
}
