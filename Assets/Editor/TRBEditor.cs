using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RollingBallController))]
public class TRBEditor : Editor
{
    private void OnSceneGUI()
    {
        RollingBallController trb = (RollingBallController)target;

        Color c = Color.green;

        Handles.color = new Color(c.r, c.g, c.b, 0.3f);
        Handles.DrawSolidArc(
            trb.transform.position,
            trb.transform.up,
            Quaternion.AngleAxis(-trb.fovAngle / 2f, trb.transform.up) * trb.transform.forward,
            trb.fovAngle,
            trb.Fov);

        Handles.color = c;
        trb.Fov = Handles.ScaleValueHandle(
            trb.Fov,
            trb.transform.position + trb.transform.forward * trb.Fov,
            trb.transform.rotation,
            3,
            Handles.SphereHandleCap,
            1); 
    }
}
