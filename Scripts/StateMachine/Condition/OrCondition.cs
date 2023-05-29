using System.Collections.Generic;

namespace BlackboardStateMachine
{
    /// <summary>
    /// /// Compound Condition that requires all of its sub-conditions to be triggered.
    /// </summary>
    public class OrCondition : Condition
    {
        private List<Condition> subConditions;

        public OrCondition(params Condition[] subConditions)
        {
            this.subConditions = new List<Condition>();
            this.subConditions.AddRange(subConditions);
        }

        public override bool TestCondition(Blackboard agentBlackboard)
        {
            foreach (Condition subCondition in subConditions)
            {
                if (subCondition.TestCondition(agentBlackboard))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
