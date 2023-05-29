/* Base class for running a flat StateMachine.
 * Requires State, StateTransition, and Condition classes to operate.
 * 
 * Code adapted from "AI for Games" by Ian Millington (3rd Edition).
 * Original C++ pseudocode translated to C# and Unity by myself (Reed Spratt).
 * 
 * Added support for "AnyState" transitions.
 * Added support for StateMachine entry and exit behaviour.
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlackboardStateMachine
{
    /// <summary>
    /// Driver class for running a StateMachine on an object.
    /// Actions are assumed to be returned to a component script for execution.
    /// </summary>
    [System.Serializable]
    public class StateMachine
    {
        public List<State> states;

        private int initialStateIdx;
        private State currentState;

        private List<StateTransition> anyStateTransitions;

        //Optional: Specifies actions when entering the state machine.
        private UnityEvent stateMachineEntryActions;

        //Optional: Specifisd actions when exiting the state machine.
        private UnityEvent stateMachineExitActions;

        public List<State> States {get {return states;}}


        public StateMachine(UnityEvent stateMachineEntryActions, UnityEvent stateMachineExitActions)
        {
            states = new List<State>();
            anyStateTransitions = new List<StateTransition>();
            initialStateIdx = 0;

            this.stateMachineEntryActions = stateMachineEntryActions;
            this.stateMachineExitActions = stateMachineExitActions;
        }

        /// <summary>
        /// Set the entry state for the StateMachine.
        /// </summary>
        /// <param name="stateIdx"></param>
        public void SetInitialState(int stateIdx)
        {
            initialStateIdx = stateIdx;
        }

        /// <summary>
        /// Add a state to the state machine.
        /// </summary>
        /// <param name="stateToAdd"></param>
        public void AddState(State stateToAdd)
        {
            states.Add(stateToAdd);
        }

        /// <summary>
        /// Add a transition within the StateMachine that can be executed from any state, i.e. "Death".
        /// </summary>
        /// <param name="destState"></param>
        /// <param name="transitionActions"></param>
        /// <param name="transitionCondition"></param>
        public void AddAnyStateTransition(State destState, UnityEvent transitionActions, Condition transitionCondition)
        {
            foreach (State machineState in states)
            {
                if (destState.Equals(machineState))
                {
                    anyStateTransitions.Add(new StateTransition(destState, transitionActions, transitionCondition));
                }
            }
        }

        /// <summary>
        /// Exit the state machine and return any final exit actions.
        /// </summary>
        /// <returns></returns>
        public List<UnityEvent> ExitStateMachine()
        {
            List<UnityEvent> exitActions = new();

            if (currentState != null)
                exitActions.Add(currentState.GetExitActions());

            currentState = null;

            if (stateMachineExitActions != null)
                exitActions.Add(stateMachineExitActions);

            return exitActions;
        }

        /// <summary>
        /// Get a list of UnityEvents with actions for an StateMachineAgent to execute.
        /// </summary>
        /// <returns>A list of UnityEvents to invoke.</returns>
        public List<UnityEvent> GetActions(Blackboard agentBlackboard)
        {
            List<UnityEvent> stateActions;

            StateTransition trigger = null;

            //Entering the StateMachine
            if (currentState == null)
            {
                currentState = states[initialStateIdx];

                stateActions = new List<UnityEvent>() {currentState.GetEntryActions() };

                if (stateMachineEntryActions != null)
                    stateActions.Insert(0, stateMachineEntryActions);
            }

            else
            {
                //Check for ANY-state transitions first
                foreach (StateTransition anyTransition in anyStateTransitions)
                {
                    if (anyTransition.IsTriggered(agentBlackboard))
                    {
                        trigger = anyTransition;
                        break;
                    }
                }

                //Check for active state transitions
                if (trigger == null)
                {
                    foreach (StateTransition transition in currentState.GetTransitions())
                    {
                        if (transition.IsTriggered(agentBlackboard))
                        {
                            trigger = transition;
                            break;
                        }
                    }
                }

                //A transition was triggered, get the actions to execute
                if (trigger != null)
                {
                    State targetState = trigger.TargetState;
                    Debug.LogFormat("Transition to new state {0}", targetState.StateName);

                    //Exit the StateMachine
                    if (trigger.TargetState == null)
                    {
                        stateActions = ExitStateMachine();
                    }

                    else
                    {
                        stateActions = new List<UnityEvent>
                        {
                            currentState.GetExitActions(),
                            trigger.TransitionActions,
                            targetState.GetEntryActions()
                        };

                        currentState = targetState;
                    }
                }

                //Return the current state's actions
                else
                {
                    stateActions = new List<UnityEvent>() { currentState.GetActions() };
                }
            }

            return stateActions;
        }
    }
}