using UnityEngine;

namespace BlackboardStateMachine
{
    /// <summary>
    /// ComparerCondition class of type Float.
    /// </summary>
    /// <typeparam name="Vector3"></typeparam>
    public class Vector3ComparerCondition: ComparerCondition<Vector3>
    {
        public Vector3ComparerCondition(Vector3 staticValue, string dynamicValue, ComparerOperator comparerOperator): base(staticValue, dynamicValue, comparerOperator)
        {
        }

        public override bool TestCondition(Blackboard agentBlackboard)
        {
            //Implementation for other operators (<, <=, >, >=) can vary depending on the use case.
            return comparerOperator switch
            {
                ComparerOperator.EQ => agentBlackboard.GetVector3Value(dynamicValue) == staticValue,
                ComparerOperator.NEQ => agentBlackboard.GetVector3Value(dynamicValue) != staticValue,
            _ => false,
            };
        }
    }
}
