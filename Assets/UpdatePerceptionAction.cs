using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdatePerception", story: "Agent checks if it sees the [Target]", category: "Action", id: "05c07ab3129a69cebbed48342eec1104")]
public partial class UpdatePerceptionAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Target;

    private AIPerception perception;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

