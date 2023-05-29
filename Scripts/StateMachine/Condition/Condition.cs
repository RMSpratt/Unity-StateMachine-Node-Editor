
namespace BlackboardStateMachine
{
    /// <summary>
    /// Base class for State Machine Transition Conditions.
    /// </summary>
    public abstract class Condition
    {
        /// <summary>
        /// Key Condition function that returns true when the Condition is met.
        /// </summary>
        /// <returns></returns>
        public abstract bool TestCondition(Blackboard agentBlackboard);
    }
}