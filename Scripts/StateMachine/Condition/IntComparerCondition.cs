
namespace BlackboardStateMachine
{
    /// <summary>
    /// ComparerCondition class of type Integer.
    /// </summary>
    /// <typeparam name="int"></typeparam>
    public class IntComparerCondition: ComparerCondition<int>
    {
        public IntComparerCondition(int staticValue, string dynamicValue, ComparerOperator comparerOperator): base(staticValue, dynamicValue, comparerOperator)
        {
        }

        public override bool TestCondition(Blackboard agentBlackboard)
        {
            return comparerOperator switch
            {
                ComparerOperator.EQ => agentBlackboard.GetIntValue(dynamicValue) == staticValue,
                ComparerOperator.LT => agentBlackboard.GetIntValue(dynamicValue) < staticValue,
                ComparerOperator.GT => agentBlackboard.GetIntValue(dynamicValue) > staticValue,
                ComparerOperator.LTE => agentBlackboard.GetIntValue(dynamicValue) <= staticValue,
                ComparerOperator.GTE => agentBlackboard.GetIntValue(dynamicValue) >= staticValue,
                ComparerOperator.NEQ => agentBlackboard.GetIntValue(dynamicValue) != staticValue,
            _ => false,
            };
        }
    }
}
