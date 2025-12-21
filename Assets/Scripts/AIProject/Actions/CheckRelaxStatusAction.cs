using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckRelaxStatus", story: "[Agent] checks if it's time to relax", category: "Action", id: "55c5564f989b7b7cbb53a934fb408f75")]
public partial class CheckRelaxStatusAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> TimeOnPatrol;
    [SerializeReference] public BlackboardVariable<States> CurrentState;
    [SerializeReference] public BlackboardVariable<float> TimeToRelaxTimer;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (TimeOnPatrol < TimeToRelaxTimer || CurrentState.Value != States.Patrol)
            return Status.Running;

        return Status.Success;
    }

}

