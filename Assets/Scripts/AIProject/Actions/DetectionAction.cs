using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Detection", story: "[Agent] checks for [enemy] detection", category: "Action", id: "b76e705902f47add429f1ee7ecd73a3e")]
public partial class DetectionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<bool> IsPlayerDetected;

    private AIPerception sensors;
    protected override Status OnStart()
    {
        sensors = Agent.Value.GetComponent<AIPerception>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (sensors.IsPlayerDetected())
        {
            IsPlayerDetected.Value = true;
            return Status.Success;
        }

        IsPlayerDetected.Value = false;
        return Status.Running;
    }

}

