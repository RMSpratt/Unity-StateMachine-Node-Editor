using UnityEngine;

namespace BlackboardStateMachine
{
    /// <summary>
    /// ComparerCondition class of type Float.
    /// </summary>
    /// <typeparam name="Vector2"></typeparam>
    public class Vector2ComparerCondition: ComparerCondition<Vector2>
    {
        public Vector2ComparerCondition(Vector2 staticValue, string dynamicValue, ComparerOperator comparerOperator): base(staticValue, dynamicValue, comparerOperator)
        {
        }

        public override bool TestCondition(Blackboard agentBlackboard)
        {
            //Implementation for other operators (<, <=, >, >=) can vary depending on the use case.
            return comparerOperator switch
            {
                ComparerOperator.EQ => agentBlackboard.GetVector2Value(dynamicValue) == staticValue,
                ComparerOperator.NEQ => agentBlackboard.GetVector2Value(dynamicValue) != staticValue,
            _ => false,
            };
        }
    }
}
