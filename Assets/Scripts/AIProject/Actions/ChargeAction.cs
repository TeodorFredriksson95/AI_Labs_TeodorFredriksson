using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Charge", story: "[Agent] charges the [Target]", category: "Action", id: "42285eb63a5f6c6bde7cf87557c983c1")]
public partial class ChargeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> IsPlayerTagged;
    [SerializeReference] public BlackboardVariable<bool> ShouldAbort;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;


    private NavMeshAgent navAgent;
    private Transform player;
    private Rigidbody prb;
    private PlayerController pc;
    protected override Status OnStart()
    {
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        player = Target.Value.transform;
        prb = Target.Value.GetComponent<Rigidbody>();
        pc = Target.Value.GetComponent<PlayerController>();

        ShouldAbort.Value = true;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsPlayerTagged.Value)
        {
            RuntimeUI.Instance.UpdateDeathsLabel();
            return Status.Success;
        }
        LastKnownPosition.Value = player.transform.position;

        Vector3 direction = player.transform.position - Agent.Value.transform.position;
        float distance = direction.magnitude;
        navAgent.speed = 10f;
        navAgent.SetDestination(player.transform.position);

        if (distance < 2f)
        {
            // This fix is insane.
            // We set the new destination to the agent itself because I want the agent to instantly stop when inside the threshold,
            // without having to make adjustments to speed, deceleration, etc, that would each have to be manually reset.
            // With this, we can just let the next flow of action set the new destination.
            navAgent.SetDestination(navAgent.transform.position);


            Vector3 knockbackDir = (Target.Value.transform.position - Agent.Value.transform.position).normalized;
            knockbackDir.y = 0;
            float knockbackForce = 8f;
            float upwardForce = 4f;

            Vector3 force = knockbackDir * knockbackForce + Vector3.up * upwardForce;
            prb.linearVelocity = Vector3.zero;
            prb.angularVelocity = Vector3.zero;

            pc.SetKnockback(force);
            IsPlayerTagged.Value = true;
            Debug.Log("Enemy was knocked back");
        }

        return Status.Running;
    }

}

