using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Perception", story: "[Agent] updates perception", category: "Action/Sensing", id: "cb7bd70b047660064cf1079a21187cc2")]
public partial class UpdatePerceptionAction : Action
{
    /// <summary>
    /// Custom Unity Behavior Action node:
    /// - Reads GuardSensors
    /// - Writes to blackboard: Target, HasLineOfSight, LastKnownPosition, TimeSinceLastSeen
    ///
    /// NOTE: Best workflow:
    /// 1) Create this node via the Behavior Graph editor (Create new -> Action),
    /// 2) Then replace the generated script content with this file.
    /// </summary>
    /// 
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
    name: "Update Perception",
    description: "Updates Target/LOS/LastKnownPosition from GuardSensors.",
    story: "Update perception and write to the blackboard.",
    category: "Action/Sensing",
    id: "b0c8b9b7-8f64-4c0c-a9a0-0d9e7c2e2fb8"
    )]
    public class UpdatePerception : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Target;
        [SerializeReference]
        public BlackboardVariable<bool> HasLineOfSight;
        [SerializeReference]
        public BlackboardVariable<Vector3> LastKnownPosition;
        [SerializeReference]
        public BlackboardVariable<float> TimeSinceLastSeen;
        protected override Status OnStart()
        {
            // Ensure we have sane defaults.
            if (TimeSinceLastSeen != null && TimeSinceLastSeen.Value < 0f)
                TimeSinceLastSeen.Value = 9999f;
            return Status.Running;
        }
        protected override Status OnUpdate()
        {
            Debug.Log("perception");

            var sensors = GameObject != null ? GameObject.GetComponent<guardSensors_Lab4>() : null;
            if (sensors == null)
            {
                // No sensors attached -> treat as "can't see anything"
                if (HasLineOfSight != null) HasLineOfSight.Value = false;
                if (TimeSinceLastSeen != null)
                    TimeSinceLastSeen.Value += Time.deltaTime;

                return Status.Success;
            }
            bool sensed = sensors.TrySenseTarget(
            out GameObject sensedTarget,
            out Vector3 sensedPos,
            out bool hasLOS
            );
            if (sensed && hasLOS)
            {
                if (Target != null) Target.Value = sensedTarget;
                if (HasLineOfSight != null) HasLineOfSight.Value = true;
                if (LastKnownPosition != null)
                    LastKnownPosition.Value = sensedPos;
                if (TimeSinceLastSeen != null)
                    TimeSinceLastSeen.Value = 0f;
            }
            else
            {
                // Keep Target as-is (we "remember" who we were chasing),
                // but mark that we don't currently have LOS.
                if (HasLineOfSight != null) HasLineOfSight.Value = false;
                if (TimeSinceLastSeen != null)
                    TimeSinceLastSeen.Value += Time.deltaTime;
            }
            // This node is a fast "service-like" update; it finishes immediately each tick.
            return Status.Success;
        }
    }
}

