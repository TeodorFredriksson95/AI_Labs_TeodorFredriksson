using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NavigateToRandomPatrolpoint", story: "[Agent] navigates to a random patrolpoint", category: "Action", id: "75cf20e61448442593dca4b0b9309586")]
public partial class NavigateToRandomPatrolpointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<GameObject>> PatrolPoints;

    private Transform randomPatrolpoint;
    private NavMeshAgent navAgent;
    protected override Status OnStart()
    {
        int randomIndex = UnityEngine.Random.Range(0, PatrolPoints.Value.Count);
        randomPatrolpoint = PatrolPoints.Value[randomIndex].transform;
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        navAgent.SetDestination(randomPatrolpoint.transform.position);

        if (navAgent.pathPending || navAgent.remainingDistance > 0.001f)
        {
            return Status.Running;
        }
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

