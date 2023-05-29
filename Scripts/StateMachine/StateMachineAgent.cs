using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using BlackboardStateMachine;

/// <summary>
/// Base class for a StateMachineAgent using a Blackboard StateMachine.
/// </summary>
[System.Serializable]
public class StateMachineAgent : MonoBehaviour
{
    public string agentName;
    
    private StateMachine agentStateMachine;
    
    private Blackboard agentBlackboard;

    [SerializeField]
    private StateMachineAgent parent;

    [SerializeField]
    [HideInInspector]
    public StateMachineGraph agentGraph;

    public List<ActionInfo> agentActions;
    
    public Blackboard AgentBlackboard => agentBlackboard;

    void BuildBlackboard()
    {
        //Need to account for parent blackboard not being created yet
        //1) Could access by ref
        //2) Could create a signal to allow the parent to retroactively assign the variable
        agentBlackboard = new Blackboard(agentGraph.agentWorldModel);
        
        if (parent != null)
        {
            agentBlackboard.SetParent(parent.agentBlackboard);
        }
    }

    /// <summary>
    /// Main builder function to convert serialized StateMachine data into a working StateMachine.
    /// </summary>
    void BuildStateMachine()
    {
        StateMachine agentStateMachine = new StateMachine(null, null);

        Dictionary<int, State> createdStates = new Dictionary<int, State>();

        //Mapping of a compound condition marker to a list of sub-conditions for creation
        Dictionary<int, List<ConditionMarker>> subConditionLists = new Dictionary<int, List<ConditionMarker>>();

        Dictionary<string, UnityEvent> agentActionLookup = new Dictionary<string, UnityEvent>();

        foreach (ActionInfo agentAction in agentActions)
        {
            agentActionLookup.Add(agentAction.actionName, agentAction.actionFuncs);
        }

        foreach (StateMarker stateDescription in agentGraph.stateMarkers)
        {
            State agentState = StateBuilder.BuildState(stateDescription, agentActionLookup);
            createdStates.Add(stateDescription.stateIdx, agentState);
            agentStateMachine.AddState(agentState);
        }

        //Organize the ConditionMarker into lists per Transition O(n)
        foreach (ConditionMarker conditionDescription in agentGraph.conditionMarkers)
        {
            if (subConditionLists.ContainsKey(conditionDescription.transitionIdx) == false)
                subConditionLists.Add(conditionDescription.transitionIdx, new List<ConditionMarker>());

            subConditionLists[conditionDescription.transitionIdx].Add(conditionDescription);
        }

        ///Build each Transition and Condition tree 
        foreach (TransitionMarker transitionDescription in agentGraph.transitionMarkers)
        {
            State startState = createdStates[transitionDescription.startStateIdx];
            State endState = createdStates[transitionDescription.endStateIdx];

            List<ConditionMarker> transitionConditionMarkers = subConditionLists[transitionDescription.transitionIdx];

            Dictionary<int, List<Condition>> subConditionList = new Dictionary<int, List<Condition>>();

            for (int i = transitionConditionMarkers.Count-1; i >= 0; i--)
            {
                ConditionMarker transitionConditionMarker = transitionConditionMarkers[i];

                if (transitionConditionMarker.conditionType == ConditionMarkerType.Comparer)
                {
                    Condition agentCondition = ConditionBuilder.BuildComparerCondition(transitionConditionMarker);

                    if (!subConditionList.ContainsKey(transitionConditionMarker.parentIdx))
                    {
                        subConditionList.Add(transitionConditionMarker.parentIdx, new List<Condition>());
                    }

                    subConditionList[transitionConditionMarker.parentIdx].Add(agentCondition);
                }

                else
                {
                    Condition agentCondition = ConditionBuilder.BuildCompoundCondition(transitionConditionMarker, subConditionList[transitionConditionMarker.conditionIdx].ToArray());
                    
                    if (!subConditionList.ContainsKey(transitionConditionMarker.parentIdx))
                        subConditionList.Add(transitionConditionMarker.parentIdx, new List<Condition>());

                    subConditionList[transitionConditionMarker.parentIdx].Add(agentCondition);
                }
            }

            StateTransition agentTransition = TransitionBuilder.BuildTransition(transitionDescription, agentActionLookup, endState, subConditionList[-1][0]);
            startState.AddTransition(agentTransition);
        }

        this.agentStateMachine = agentStateMachine;
    }

    void Awake()
    {
        //Create Blackboard first
        BuildBlackboard();
        BuildStateMachine();
    }

    public List<UnityEvent> GetAgentActions()
    {
        return agentStateMachine.GetActions(agentBlackboard);        
    }

    /// <summary>
    /// Editor utility function.
    /// </summary>
    /// <returns></returns>
    public string[] GetActionNames()
    {
        string[] actionNames = new string[agentActions.Count];

        for (int i = 0; i < agentActions.Count; i++)
        {
            actionNames[i] = agentActions[i].actionName;
        }

        return actionNames;
    }
}

