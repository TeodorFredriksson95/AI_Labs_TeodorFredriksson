using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.Rendering.DebugUI;

public class RendezvousCoordinator : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent m_Agent;
    private BlackboardVariable<StateEventChannel> m_stateEventChannelBBV;
    private BlackboardVariable<HelperWannaChillEvent> HelperWannaChillChannel;
    private BlackboardVariable<TrbWannaChillEvent> TRBWannaChillChannel;
    private BlackboardVariable<HelperInterruptedChannel> HelperInterruptedChannel;

    private BlackboardVariable<States> m_stateBBV;

    [SerializeField] RollingBallController trb;
    public bool HelperReady { get; private set; }
    public bool TRBReady { get; private set; }

    public bool BothReady => HelperReady && TRBReady;

    public event System.Action OnBothReady;

    private void OnEnable()
    {
        if (m_Agent.GetVariable("StateEventChannel", out m_stateEventChannelBBV))
            m_stateEventChannelBBV.Value.Event += OnStateEvent;

        if (m_Agent.GetVariable("State", out m_stateBBV))
            m_stateBBV.OnValueChanged += OnStateValueChanged;

        if (m_Agent.GetVariable("HelperChillEvent", out HelperWannaChillChannel))
            HelperWannaChillChannel.Value.Event += OnHelperWannaChillEvent;

        m_Agent.GetVariable("TRBChillEvent", out TRBWannaChillChannel);

        if (m_Agent.GetVariable("HelperInterruptedEvent", out HelperInterruptedChannel))
            HelperInterruptedChannel.Value.Event += OnHelperInterrupted;

    }

    private void OnDisable()
    {
        if (m_stateEventChannelBBV != null)
            m_stateEventChannelBBV.Value.Event -= OnStateEvent;
        if (m_stateBBV != null)
            m_stateBBV.OnValueChanged -= OnStateValueChanged;
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
        trb.BreakIsOver();
    }

    private void OnHelperWannaChillEvent()
    {
        if (trb.CanTRBTransitionToChill())
        {
            trb.GoOnBreak();
            TRBWannaChillChannel.Value.SendEventMessage();
        }
    }

    private void OnStateEvent(States value)
    {
        //TRBReadyChannel.Value.SendEventMessage(States.Patrol);


        //// React to event
        //Debug.Log("OnStateEvent value: " + value);

        //// State Event is only sent *if* the Helper agent is ready to go to chill point.
        //// If check is redundant, but in case logic ever changes
        //if (value == States.GoToChillPoint)
        //    IsHelperReady(isReady: true);
        //else
        //    IsHelperReady(isReady: false);
    }


    private void OnStateValueChanged()
    {
        //if (m_stateBBV.Value == States.GoToChillPoint)
        //{
        //    trb.helperAgentState = States.GoToChillPoint;
        //    // Check TRB if its ready. assign a local member the boolean value. use it in update check.
        //    TRBReady = trb.CanTRBTransitionToChill();
        //    IsHelperReady(isReady: true);
        //    m_stateEventChannelBBV.Value.SendEventMessage(States.Alert);

        //    Check();
        //}
        //else
        //{
        //    trb.helperAgentState = States.Patrol;
        //    IsHelperReady(isReady: false);
        //}

        //// React to state change
        //Debug.Log("OnStateValueChanged. State is: " + m_stateBBV);

        ////if (m_stateBBV.Value != States.GoToChillPoint)
        ////    IsTRBReady(false);

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
        if (isReady)
            TRBReady = true;
        else
        {
            TRBReady = false;
            trb.helperAgentState = States.Patrol; // We need to set this to anything other than GoToChillPoint, to gatekeep TRBs FSM.
            trb.CurrentState = TRBState.Patrolling;
        }

    }

    private void Check()
    {
        if (BothReady)
            m_stateEventChannelBBV.Value.SendEventMessage(States.Alert);

        //m_Agent.SetVariableValue("ShouldMoveToSafePoint", true);
    }
}
