namespace BlackboardStateMachine
{
    /// <summary>
    /// ComparerCondition class of type Float.
    /// </summary>
    /// <typeparam name="bool"></typeparam>
    public class BoolComparerCondition: ComparerCondition<bool>
    {
        public BoolComparerCondition(bool staticValue, string dynamicValue, ComparerOperator comparerOperator): base(staticValue, dynamicValue, comparerOperator)
        {
        }

        public override bool TestCondition(Blackboard agentBlackboard)
        {
            return comparerOperator switch
            {
                ComparerOperator.EQ => agentBlackboard.GetBoolValue(dynamicValue) == staticValue,
                ComparerOperator.NEQ => agentBlackboard.GetBoolValue(dynamicValue) != staticValue,
            _ => false,
            };
        }
    }
}
