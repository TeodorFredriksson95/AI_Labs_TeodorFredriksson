using Unity.VisualScripting;
using UnityEngine;
using static RollingBallController;

public class AIPerception : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Detection range")]
    [Range(0, 360)] public float fovAngle = 180f;
    public float Fov;

    private bool isPlayerDetected;
    Vector3 targetPos;
    PlayerController target;
    private void Awake()
    {
        target = FindFirstObjectByType<PlayerController>();
    }

    public bool IsPlayerDetected()
    {
        Collider[] targetsInFov = Physics.OverlapSphere(transform.position, Fov);

        foreach (Collider c in targetsInFov)
        {
            if (c.CompareTag("Player"))
            {
                float signedAngle = Vector3.Angle(transform.forward, c.transform.position - transform.position);

                if (Mathf.Abs(signedAngle) < fovAngle / 2)
                {
                    isPlayerDetected = true;
                    targetPos = c.transform.position;
                    return true;
                }
            }
        }

        isPlayerDetected = false;
        targetPos = Vector3.zero;
        return false;
    }

    public bool IsPlayerInLoS()
    {
        if (!isPlayerDetected) return false;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 target = targetPos + Vector3.up * 0.5f;
        Vector3 dir = target - origin;

        if (Physics.Raycast(origin, dir, out hit))
        {
            if (hit.transform == player) return true;
        }

        return false;
    }
}
