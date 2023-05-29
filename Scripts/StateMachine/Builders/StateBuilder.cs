using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BlackboardStateMachine;

/// <summary>
/// Builder class to create executable States from design-level StateMarker class instances.
/// </summary>
public static class StateBuilder
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
    /// Builds a UnityEvent for capturing a state's action list onEntry, onActive, or onExit.
    /// </summary>
    /// <param name="actionEvent">The state's UnityEvent</param>
    /// <param name="stateActionNames">The list of possible actions from the agent.</param>
    /// <param name="agentActions">A mapping of an agent's action names to actions.</param>
    static void BuildStateActionEvent(UnityEvent actionEvent, List<string> stateActionNames, Dictionary<string, UnityEvent> agentActions)
    {
        foreach (string actionName in stateActionNames)
        {
            if (agentActions.ContainsKey(actionName))
            {
                CopyEventListeners(agentActions[actionName], actionEvent);
            }
        }
    }

    /// <summary>
    /// Builds a State instance suited for a StateMachine using a serialized StateMarker.
    /// </summary>
    /// <param name="stateDescription"></param>
    /// <param name="agentActions">A mapping of an agent's action names to actions.</param>
    /// <returns></returns>
    public static State BuildState(StateMarker stateDescription, Dictionary<string, UnityEvent> agentActions)
    {
        State newAgentState = null;
        UnityEvent entryActions = new UnityEvent();
        UnityEvent regActions = new UnityEvent(); 
        UnityEvent exitActions = new UnityEvent();

        BuildStateActionEvent(entryActions, stateDescription.entryActions, agentActions);
        BuildStateActionEvent(regActions, stateDescription.regActions, agentActions);
        BuildStateActionEvent(exitActions, stateDescription.exitActions, agentActions);

        newAgentState = new State(stateDescription.stateName, entryActions, regActions, exitActions);

        return newAgentState;
    }
}
