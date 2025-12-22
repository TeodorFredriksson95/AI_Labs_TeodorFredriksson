using Assets.Scripts.AIProject;
using System;
using Unity.Behavior;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForTRB", story: "[Agent] waits for TRB before officially going on break", category: "Action", id: "b287a16e68bb4ef20c74335c048ecf17")]
public partial class WaitForTrbAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        bool isTRBPresent = NPCUtility.IsTargetDetected(Agent.Value.transform, 5f, "TRB");

        return isTRBPresent ? Status.Success : Status.Running;
    }

}
