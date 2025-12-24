using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateLOS", story: "[Agent] checks [enemy] LOS", category: "Action", id: "d7451355d91116510054852b5dbf4ce0")]
public partial class UpdateLosAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<bool> HasLineOfSight;
    [SerializeReference] public BlackboardVariable<float> TimeSinceLastSeen;

    private AIPerception sensors;
    protected override Status OnStart()
    {
        sensors = Agent.Value.GetComponent<AIPerception>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("updating enemy los");    
        if (sensors.IsPlayerInLoS())
        {
            HasLineOfSight.Value = true;
            TimeSinceLastSeen.Value = 0f;
            return Status.Running;
        }
        else
        {
            HasLineOfSight.Value = false;
            TimeSinceLastSeen.Value += Time.deltaTime;
        }


        return Status.Success;
    }

}

