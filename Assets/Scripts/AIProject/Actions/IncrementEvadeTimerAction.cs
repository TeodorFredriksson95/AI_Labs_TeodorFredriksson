using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IncrementEvadeTimer", story: "Increments the [evade] timer", category: "Action", id: "37b2fe18621efc451d8255f87c27115a")]
public partial class IncrementEvadeTimerAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Evade;
    [SerializeReference] public BlackboardVariable<float> EvadeCounter;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Evade.Value < EvadeCounter.Value)
        {
            Evade.Value += Time.deltaTime;
            return Status.Running;
        }
        return Status.Success;
    }

}

