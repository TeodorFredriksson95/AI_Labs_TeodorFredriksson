using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IncrementPatrolTimer", story: "Increments the amount of [patrolTime] the agent has patrolled", category: "Action", id: "fcc424112814cf3752d50389dc1787c4")]
public partial class IncrementPatrolTimerAction : Action
{
    [SerializeReference] public BlackboardVariable<float> PatrolTime;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        PatrolTime.Value += Time.deltaTime;
        return Status.Running;
    }

}

