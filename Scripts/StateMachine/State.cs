using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlackboardStateMachine
{
    /// <summary>
    /// State representation for a StateMachine.
    /// </summary>
    public class State
    {
        string stateName;

        List<StateTransition> stateTransitions;

        UnityEvent entryActions;
        UnityEvent regActions;
        UnityEvent exitActions;

        public string StateName => stateName;

        public UnityEvent GetActions() => regActions;
        public UnityEvent GetEntryActions() => entryActions;
        public UnityEvent GetExitActions() => exitActions;

        public List<StateTransition> GetTransitions() => stateTransitions;

        public void AddTransition(StateTransition newTransition)
        {
            stateTransitions.Add(newTransition);
        }
        
        public State(string stateName, UnityEvent entryActions = null, UnityEvent regActions = null, UnityEvent exitActions = null)
        {
            this.stateName = stateName;
            this.regActions = regActions;
            this.entryActions = entryActions;
            this.exitActions = exitActions;
            stateTransitions = new List<StateTransition>();
        }
    }
}