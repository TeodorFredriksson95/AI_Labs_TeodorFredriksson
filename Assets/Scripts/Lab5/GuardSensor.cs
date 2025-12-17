using Unity.VisualScripting;
using UnityEngine;

public class GuardSensor : MonoBehaviour
{
    public Transform player;
    public float viewRange = 10f;
    public LayerMask occluders = ~0;
    public bool useLineOfSightRaycast = true;

    public bool SeesPlayer { get; set; }

    private void Update()
    {
        SeesPlayer = false;
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > viewRange) return;

        if (!useLineOfSightRaycast)
        {
            SeesPlayer = true;
            return; // why early return here?
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 target = player.position + Vector3.up * 0.5f;
        Vector3 dir = target - origin;
        float len = dir.magnitude;

        if (len < 0.001f)
        {
            SeesPlayer = true;
            return;
        }

        // why dir / len as direction?
        if (Physics.Raycast(origin, dir / len, out RaycastHit hit, len, occluders))
        {
            if (hit.transform == player) SeesPlayer = true;
        }
    }

}
