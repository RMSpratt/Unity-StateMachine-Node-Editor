using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BlackboardStateMachine;

/// <summary>
/// Builder class to create executable Transitions from design-level TransitionMarker class instances.
/// </summary>
public static class TransitionBuilder
{
    /// <summary>
    /// Copy a UnityEvent's action listeners to a new UnityEvent.
    /// </summary>
    /// <param name="sourceEvent">The source event to be copied.</param>
    /// <param name="targetEvent">The target event as the copy.</param>
    static void CopyEventListeners(UnityEvent sourceEvent, UnityEvent targetEvent)
    {
        // Get the persistent call group of the source event
        for (int i = 0; i < sourceEvent.GetPersistentEventCount(); i++)
        {
            Object sourceTarget = sourceEvent.GetPersistentTarget(i);
            string sourceMethod = sourceEvent.GetPersistentMethodName(i);

            //Credit: https://forum.unity.com/threads/copying-unityevent-information-using-reflection.427226/
            UnityAction actionCopy = System.Delegate.CreateDelegate(typeof(UnityAction), sourceTarget, sourceMethod) as UnityAction;
            
            // Add the listener to the target event
            targetEvent.AddListener(actionCopy);
        }
    }

    /// <summary>
    /// Builds a StateTransition instance suited for a StateMachine using a serialized TransitionMarker.
    /// </summary>
    /// <param name="transitionDescription">The serialized information for the transition.</param>
    /// <param name="agentActions">A mapping of an agent's action names to actions.</param>
    /// <param name="targetState">The target state of the transition.</param>
    /// <param name="transitionCondition">The condition for triggering the transition.</param>
    /// <returns></returns>
    public static StateTransition BuildTransition(TransitionMarker transitionDescription, Dictionary<string, UnityEvent> agentActions, State targetState, Condition transitionCondition)
    {
        StateTransition newAgentTransition = null;
        UnityEvent transitionActions = new UnityEvent();

        foreach (string actionName in transitionDescription.transitionActions)
        {
            if (agentActions.ContainsKey(actionName))
            {
                CopyEventListeners(agentActions[actionName], transitionActions);
            }
        }

        newAgentTransition = new StateTransition(targetState, transitionActions, transitionCondition);
        return newAgentTransition;
    }
}
