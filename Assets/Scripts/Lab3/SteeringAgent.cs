using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Security;

public class SteeringAgent : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5f;
    public float maxForce = 10f; // Limits how fast we can change direction (turning radius)

    [Header("Arrive")]
    [SerializeField] private float slowingRadius = 4.0f;

    [Header("Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 5f;

    [Header("Weights")]
    public float arriveWeight = 1f;
    public float separationWeight = 1f;

    [Header("Seek / Arrive")]
    public Transform target;
    public static List<SteeringAgent> allAgents = new List<SteeringAgent>();

    [Header("Debug")]
    public bool drawDebug = true;

    private Vector3 velocity = Vector3.zero;


    private void OnEnable()
    {
        if (this != null)
            allAgents.Add(this);
    }

    private void OnDisable()
    {
        allAgents.Remove(this);
    }

    private void Start()
    {
        target = FindFirstObjectByType<PlayerController>().transform;
    }

    private void Update()
    {
        // Get distance from player
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // 1. Calculate Steering Force
        Vector3 steering = Vector3.zero;

        if (target != null)
            steering += Arrive(target.position, slowingRadius) * arriveWeight;

        if (allAgents.Count > 1)
            steering += Separation(separationRadius, separationStrength) * separationWeight;

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
        Vector3 toTarget = targetPos - transform.position;
        float dist = toTarget.magnitude;

        if (Mathf.Approximately(dist, 0))
            return Vector3.zero;

        float desiredSpeed = maxSpeed;

        // Ramp down speed if within radius
        if (dist < slowRadius)
            desiredSpeed = maxSpeed * (dist / (slowRadius));

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
            if (distance > 0f && distance < separationRadius)
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
            force *= separationStrength;
        }

        Vector3 properForce = new Vector3(force.x, 0f, force.z);

        return properForce;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }

}
