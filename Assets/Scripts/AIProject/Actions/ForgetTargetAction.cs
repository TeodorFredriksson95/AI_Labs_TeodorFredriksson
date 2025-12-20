using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ForgetTarget", story: "Forget the [Target] and reset perception flags", category: "Action", id: "632b775cd8c4c9bb8c8a1314dcbb42a8")]
public partial class ForgetTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> TimeSinceLastSeen;
    [SerializeReference] public BlackboardVariable<bool> IsPlayerDetected;
    [SerializeReference] public BlackboardVariable<bool> HasLineOfSight;

    protected override Status OnStart()
    {
        TimeSinceLastSeen.Value = 0f;
        IsPlayerDetected.Value = false;
        HasLineOfSight.Value = false;
        Debug.Log("Perception of enemy was forgotten");
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

}

