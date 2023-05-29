using UnityEngine;

namespace BlackboardStateMachine
{
    /// <summary>
    /// ComparerCondition class of type Float.
    /// </summary>
    /// <typeparam name="float"></typeparam>
    public class FloatComparerCondition: ComparerCondition<float>
    {
        public FloatComparerCondition(float staticValue, string dynamicValue, ComparerOperator comparerOperator): base(staticValue, dynamicValue, comparerOperator)
        {
        }

        public override bool TestCondition(Blackboard agentBlackboard)
        {
            //A leeway factor is defined for direct equality comparisons.
            return comparerOperator switch
            {
                ComparerOperator.EQ => Mathf.Abs(agentBlackboard.GetFloatValue(dynamicValue) - staticValue) <= 0.01f,
                ComparerOperator.LT => agentBlackboard.GetFloatValue(dynamicValue) < staticValue,
                ComparerOperator.GT => agentBlackboard.GetFloatValue(dynamicValue) > staticValue,
                ComparerOperator.LTE => agentBlackboard.GetFloatValue(dynamicValue) <= staticValue,
                ComparerOperator.GTE => agentBlackboard.GetFloatValue(dynamicValue) >= staticValue,
                ComparerOperator.NEQ => Mathf.Abs(agentBlackboard.GetFloatValue(dynamicValue) - staticValue) > 0.01f,
            _ => false,
            };
        }
    }
}
