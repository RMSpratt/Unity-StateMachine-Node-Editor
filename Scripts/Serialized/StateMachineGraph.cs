using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Graph representation of an Agent's StateMachine.
/// </summary>
public class StateMachineGraph : ScriptableObject
{
    public string agentName;

    [SerializeField]
    public WorldModel agentWorldModel;

    #region StateMachine Information
    public List<StateMarker> stateMarkers;
    public List<TransitionMarker> transitionMarkers;
    public List<ConditionMarker> conditionMarkers;

    public int numCreatedStates = 0;
    public int numCreatedTransitions = 0;
    public int numCreatedConditions = 0;
    #endregion
}
