using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToTransform", story: "[Agent] moves to [Transform] location", category: "Action", id: "3ae341ca7a6cf907ea04ef8007cf33f2")]
public partial class MoveToTransformAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Transform;
    [SerializeReference] private BlackboardVariable<TrbDetectedEnemy> TRBDetectedEnemyChannel;

    private NavMeshAgent navAgent;
    private Transform safePoint;

    protected override Status OnStart()
    {
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        safePoint = Transform.Value;
        navAgent.autoBraking = true;

        navAgent.speed = 5f;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        navAgent.SetDestination(safePoint.position);


        if (!navAgent.pathPending || navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
            {
                return Status.Success;
            }
        }

        return Status.Running;
    }


}

