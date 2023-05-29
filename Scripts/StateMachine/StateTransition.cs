using UnityEngine.Events;

namespace BlackboardStateMachine
{
    /// <summary>
    /// /// Representation of a Transition (link) between two States in a StateMachine.
    /// </summary>
    public class StateTransition
    {
        Condition condition;
        State targetState;
        UnityEvent transitionActions;
    
        public State TargetState => targetState;
        public UnityEvent TransitionActions => transitionActions;

        public StateTransition (State targetState, UnityEvent transitionActions, Condition condition)
        {
            this.targetState = targetState;
            this.transitionActions = transitionActions;
            this.condition = condition;
        }

        /// <summary>
        /// Check if the Transition is triggered according to its condition.
        /// </summary>
        /// <returns></returns>
        public bool IsTriggered(Blackboard agentBlackboard)
        {
            return condition.TestCondition(agentBlackboard);
        }
    }
}
