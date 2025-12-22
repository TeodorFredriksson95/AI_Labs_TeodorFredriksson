using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.Rendering.DebugUI;

public class RendezvousCoordinator : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent m_Agent;
    private BlackboardVariable<HelperWannaChillEvent> HelperWannaChillChannel;
    private BlackboardVariable<TrbWannaChillEvent> TRBWannaChillChannel;
    private BlackboardVariable<HelperInterruptedChannel> HelperInterruptedChannel;
    private BlackboardVariable<bool> IsHelperReadyToChill;

    private BlackboardVariable<States> m_stateBBV;

    [SerializeField] RollingBallController trb;
    public bool HelperReady { get; private set; }
    public bool TRBReady { get; private set; }

    public bool BothReady => IsHelperReadyToChill.Value && TRBReady;

    public event System.Action OnBothReady;

    private void OnEnable()
    {

        if (m_Agent.GetVariable("HelperChillEvent", out HelperWannaChillChannel))
            HelperWannaChillChannel.Value.Event += OnHelperWannaChillEvent;

        m_Agent.GetVariable("TRBChillEvent", out TRBWannaChillChannel);

        if (m_Agent.GetVariable("HelperInterruptedEvent", out HelperInterruptedChannel))
            HelperInterruptedChannel.Value.Event += OnHelperInterrupted;

        m_Agent.GetVariable("IsHelperReadyToChill", out IsHelperReadyToChill);

    }

    private void OnDisable()
    {
        if (HelperWannaChillChannel != null)
            HelperWannaChillChannel.Value.Event -= OnHelperWannaChillEvent;
    }

    private void Update()
    {
        // your custom logic

        // Send event to the event channel of the referenced agent.
        // Only this instance of agent will receive it (except if the BlackboardVariable is 'Shared').
        //m_stateEventChannelBBV.Value.SendEventMessage(States.Alert);

        if (BothReady)
        {
            m_Agent.SetVariableValue("ShouldMoveToSafepoint", true); // Need to set the gatekeeping boolean in H:A's behavior graph
                                                                     // so it can move to the safepoint.

            trb.CurrentState = TRBState.Chilling; // Set the set of TRB to allow proper flow in TRB FSM.
        }
        else
            m_Agent.SetVariableValue("ShouldMoveToSafePoint", false);
    }

    private void OnHelperInterrupted()
    {
        //TRBReady = false;
        IsHelperReadyToChill.Value = false;
        trb.BreakIsOver();
    }

    private void OnHelperWannaChillEvent()
    {
        //HelperReady = true;
        IsHelperReadyToChill.Value = true;
        CheckReadyStatus();
        //if (trb.CanTRBTransitionToChill())
        //{
        //    trb.GoOnBreak();
        //    Debug.Log("Helper goes on break");
        //}
    }

    public void IsHelperReady(bool isReady)
    {
        if (isReady)
        {
            HelperReady = true;
            trb.helperAgentState = States.GoToChillPoint;
        }
        else
        {
            HelperReady = false;
            trb.helperAgentState = States.Patrol;
        }
    }

    public void IsTRBReady(bool isReady)
    {
        TRBReady = isReady;

        CheckReadyStatus();
    }

    void CheckReadyStatus()
    {
        if (BothReady)
        {
            trb.GoOnBreak();
            m_Agent.SetVariableValue("ShouldMoveToSafepoint", true); // Need to set the gatekeeping boolean in H:A's behavior graph
            TRBWannaChillChannel.Value.SendEventMessage();
            Debug.Log("asgag");
        }

    }
}
