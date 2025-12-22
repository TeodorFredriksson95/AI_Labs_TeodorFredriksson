using Assets.Scripts.AIProject;
using System;
using Unity.Behavior;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "OnBreakAction", story: "[Agent] takes a break for [duration] seconds and jumps up and down", category: "Action", id: "d7bf075c508ba0bf3566388b0eaf080f")]
public partial class OnBreakAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Duration;
    [SerializeReference] private BlackboardVariable<TrbDetectedEnemy> TRBDetectedEnemyChannel;

    private NPCJumpController jumpController;
    private NavMeshAgent navAgent;
    private Transform npcTransform;

    private float jumpTimerCounter = 0f;
    private float jumpTimer = 3f;
    private float durationCounter;

    private bool shouldAbort;

    protected override Status OnStart()
    {
        durationCounter = 0f;
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        npcTransform = Agent.Value.GetComponent<Transform>();
        jumpController = new NPCJumpController(navAgent, npcTransform, NPCUtility.gravity);


        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        durationCounter += Time.deltaTime;

        if (shouldAbort)    
            return Status.Failure;

        if (durationCounter >= Duration.Value && jumpController.isGrounded)
            return Status.Success;


        if (jumpTimerCounter >= jumpTimer)
        {

            if (!jumpController.isJumping && jumpController.isGrounded)
                jumpController.StartJump(5f);

            if (jumpController.isJumping)
            {
                jumpController.verticalVelocity += NPCUtility.gravity * Time.deltaTime;

                npcTransform.position += Vector3.up * jumpController.verticalVelocity * Time.deltaTime;

                if (npcTransform.position.y <= jumpController.groundY)
                {
                    jumpController.Land();
                    jumpTimerCounter = 0f;

                    return Status.Running;
                }

            }
        }

        jumpTimerCounter += Time.deltaTime;
        return Status.Running;
    }




}

