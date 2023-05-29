namespace BlackboardStateMachine
{
    /// <summary>
    /// ComparerCondition class of type Float.
    /// </summary>
    /// <typeparam name="string"></typeparam>
    public class StringComparerCondition: ComparerCondition<string>
    {
        public StringComparerCondition(string staticValue, string dynamicValue, ComparerOperator comparerOperator): base(staticValue, dynamicValue, comparerOperator)
        {
        }

        public override bool TestCondition(Blackboard agentBlackboard)
        {
            return comparerOperator switch
            {
                ComparerOperator.EQ => agentBlackboard.GetStringValue(dynamicValue) == staticValue,
                ComparerOperator.NEQ => agentBlackboard.GetStringValue(dynamicValue) != staticValue,
            _ => false,
            };
        }
    }
}
