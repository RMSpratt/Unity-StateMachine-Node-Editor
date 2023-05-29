
namespace BlackboardStateMachine
{
    //Comparison operators used by Comparer Conditions.
    public enum ComparerOperator
    {
        EQ,
        LT,
        GT,
        LTE,
        GTE,
        NEQ
    }

    /// <summary>
    /// Base ComparerCondition class to evaluate a static value against a dynamic WorldModel value.
    /// </summary>
    /// <typeparam name="T">Type of the variables to compare.</typeparam>
    public abstract class ComparerCondition<T> : Condition
    {
        protected T staticValue;
        protected string dynamicValue;
        protected ComparerOperator comparerOperator;

        public ComparerCondition(T staticValue, string dynamicValue, ComparerOperator comparerOperator)
        {
            this.staticValue = staticValue;
            this.dynamicValue = dynamicValue;
            this.comparerOperator = comparerOperator;
        }
    }
}
