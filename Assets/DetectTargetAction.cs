using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Rendering;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectTarget", story: "[Agent] detects [Target]", category: "Action", id: "640f80c2ab2ac9b2bb2b5dd4679168ce")]
public partial class DetectTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> EnemySpotted;
    AIPerception sensor;
    protected override Status OnStart()
    {
        sensor = Agent.Value.GetComponent<AIPerception>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        if (sensor.IsPlayerDetected())
        {
            EnemySpotted.Value = true;
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

