using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateLastKnownPosition", story: "[Agent] updates last known [enemy] position", category: "Action", id: "64f01dc1d818a60c246be969688a2b86")]
public partial class UpdateLastKnownPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;

    protected override Status OnStart()
    {

        LastKnownPosition.Value = Enemy.Value.transform.position;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

}

