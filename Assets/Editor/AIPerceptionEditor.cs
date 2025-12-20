using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AIPerception))]
public class AIPerceptionEditor : Editor
{
    private void OnSceneGUI()
    {
        AIPerception perceptionComponent = (AIPerception)target;

        Color c = Color.green;

        Handles.color = new Color(c.r, c.g, c.b, 0.3f);
        Handles.DrawSolidArc(
            perceptionComponent.transform.position,
            perceptionComponent.transform.up,
            Quaternion.AngleAxis(-perceptionComponent.fovAngle / 2f, perceptionComponent.transform.up) * perceptionComponent.transform.forward,
            perceptionComponent.fovAngle,
            perceptionComponent.Fov);

        Handles.color = c;
        perceptionComponent.Fov = Handles.ScaleValueHandle(
            perceptionComponent.Fov,
            perceptionComponent.transform.position + perceptionComponent.transform.forward * perceptionComponent.Fov,
            perceptionComponent.transform.rotation,
            3,
            Handles.SphereHandleCap,
            1);
    }
}
