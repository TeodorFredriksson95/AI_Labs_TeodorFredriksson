using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Security;
using UnityEngine.AI;
using UnityEngine.Splines;

public class SteeringAgent : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5f;
    public float maxForce = 10f; // Limits how fast we can change direction (turning radius)

    [Header("Arrive")]
    [SerializeField] private float slowingRadius = 4.0f;
    public float arriveWeight = 1f;

    [Header("Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 5f;
    public float separationWeight = 1f;

    [Header("Cohesion")]
    public float cohesionRadius = 1.5f;
    public float cohesionWeight = 1f;

    [Header("Alignment")]
    public float alignmentRadius = 1.5f;
    public float alignmentWeight = 1f;

    [Header("Seek / Arrive")]
    public Transform target;
    public static List<SteeringAgent> allAgents = new List<SteeringAgent>();

    public List<Transform> patrolPoints;
    private int destPoint = 0;
    private int pathIndex = 0;

    [Header("Debug")]
    public bool drawDebug = true;
    private NavMeshPath path;

    private Vector3 velocity = Vector3.zero;

    private NavMeshAgent agent;

    private void OnEnable()
    {
        if (this != null)
        {
            allAgents.Add(this);
        }
    }

    private void OnDisable()
    {
        allAgents.Remove(this);
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        ComputePathToWaypoint();
        target = FindFirstObjectByType<PlayerController>().transform;
    }



    private void Update()
    {
        transform.rotation = Quaternion.identity;

        // Get distance from player
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // 1. Calculate Steering Force
        Vector3 steering = Vector3.zero;

        steering += Arrive(target.position, slowingRadius) * arriveWeight;

        if (allAgents.Count > 1)
        {
            steering += Separation(separationRadius, separationStrength) * separationWeight;
            steering += Cohesion(cohesionRadius) * cohesionWeight;
            steering += Alignment(cohesionRadius) * alignmentWeight;
        }

        // 2. Limit Steering (Truncate)
        // This prevents agent from turning instantly
        steering = Vector3.ClampMagnitude(steering, maxForce);


        // 3. Apply Steering to Velocity (Integration)
        // Acceleration = Force / Mass. ( We assume Mass = 1).
        // Velocity Change = Acceleration * Time.
        velocity += steering * Time.deltaTime;

        // 4. Limit Velocity
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // 5. Move Agent
        if (distanceToPlayer > 1.5f)
            transform.position += velocity * Time.deltaTime;

        // 6. Face Movement Direction
        if (velocity.sqrMagnitude < 0.0001)
            transform.forward = velocity.normalized;

    }

    #region Behaviours
    void GoToNextPoint()
    {
        if (patrolPoints.Count == 0) return;

        //agent.destination = patrolPoints[destPoint].position;
        agent.SetDestination(patrolPoints[destPoint].position);
        destPoint = (destPoint + 1) % patrolPoints.Count;
    }

    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;

        // Stop steering if we arrived at target
        if (toTarget.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        // Desired Velocity: Full speed towards target
        Vector3 desired = toTarget.normalized * maxSpeed;

        // Reynold's Steering Formula
        return desired - velocity;

    }



    public Vector3 Arrive(Vector3 targetPos, float slowRadius)
    {
        if (path.corners.Length == 0) return Vector3.zero;

        //Vector3 toTarget = patrolPoints[destPoint].position - transform.position;
        Vector3 corner = path.corners[pathIndex];
        Vector3 toTarget = corner - transform.position;

        float dist = toTarget.magnitude;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.yellow);
        }


        //if (Mathf.Approximately(dist, 0))
        //    return Vector3.zero;

        float desiredSpeed = maxSpeed;
        bool isFinalCorner = (pathIndex == path.corners.Length - 1);

        // Ramp down speed if within radius
        if (isFinalCorner && dist < slowRadius)
            desiredSpeed = maxSpeed * (dist / (slowRadius));

        if (dist < 0.1f)
        {
            pathIndex++;
            if (pathIndex >= path.corners.Length)
            {
                destPoint = (destPoint + 1) % patrolPoints.Count;
                ComputePathToWaypoint();
                return Vector3.zero;
            }

        }

        Vector3 desired = toTarget.normalized * desiredSpeed;

        return desired - velocity;
    }

    public Vector3 Separation(float radius, float strength)
    {
        Vector3 force = Vector3.zero;
        int neighbourCount = 0;

        foreach (SteeringAgent other in allAgents)
        {
            if (other == this) continue;

            Vector3 toMe = transform.position - other.transform.position;
            float distance = toMe.magnitude;

            // If other agent is within this agents personal space
            if (distance > 0f && distance < radius)
            {
                // Weight: 1/distance means closer neighbours push MUCH harder
                force += toMe.normalized / distance;
                neighbourCount++;
            }
        }

        if (neighbourCount > 0)
        {
            force /= neighbourCount; // Average direction;

            // Covert "move away" direction into a steering force
            force = force.normalized * maxSpeed;
            force = force - velocity;
            force *= strength;
        }

        Vector3 properForce = new Vector3(force.x, 0f, force.z);

        return properForce;
    }

    public Vector3 Cohesion(float cohesianRadius)
    {
        Vector3 positionsSum = Vector3.zero;
        int neighbourCount = 0;

        foreach (SteeringAgent other in allAgents)
        {
            if (other == this) continue;

            Vector3 toMe = transform.position - other.transform.position;
            float distance = toMe.magnitude;

            // If other agent is within this agents personal space
            if (distance > 0f && distance < cohesianRadius)
            {
                positionsSum += other.transform.position;
                neighbourCount++;
            }
        }

        if (neighbourCount > 0)
        {
            positionsSum /= neighbourCount;
            return Seek(positionsSum);
        }

        return Vector3.zero;
    }

    public Vector3 Alignment(float alignmentRadius)
    {
        Vector3 velocitySum = Vector3.zero;
        int neighbourCount = 0;

        foreach (SteeringAgent other in allAgents)
        {
            if (other == this) continue;

            Vector3 toMe = transform.position - other.transform.position;
            float distance = toMe.magnitude;

            if (distance > 0 && distance < alignmentRadius)
            {
                velocitySum += other.velocity;
                neighbourCount++;
            }
        }

        Vector3 averageVelocity = Vector3.zero;
        Vector3 desired = Vector3.zero;

        if (neighbourCount > 0)
        {
            averageVelocity = velocitySum / neighbourCount;
            desired = averageVelocity.normalized * maxSpeed;
            desired.y = 0;
            return desired - velocity;
        }

        return Vector3.zero;

    }

    #endregion

    private void ComputePathToWaypoint()
    {
        pathIndex = 0;
        NavMesh.CalculatePath(transform.position, patrolPoints[destPoint].position, NavMesh.AllAreas, path);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }

}
