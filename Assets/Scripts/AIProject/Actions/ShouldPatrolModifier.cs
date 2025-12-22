using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShouldPatrol", story: "Checks if the agent should patrol", category: "Flow", id: "467fd2de5fd4d1e4011f2a5ddc48343c")]
public partial class ShouldPatrolModifier : Modifier
{

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

