using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackboardStateMachine;

/// <summary>
/// Builder class to create runnable Conditions from design-level ConditionMarker class instances.
/// </summary>
public static class ConditionBuilder
{
    /// <summary>
    /// Utility function to convert a ConditionMarker ComparerOperator to the expected type.
    /// </summary>
    /// <param name="markerOperator">The ComparerConditionOperator to be converted</param>
    /// <returns></returns>
    static ComparerOperator ConvertComparerOperator(ComparerConditionOperator markerOperator)
    {
        return markerOperator switch
        {
            ComparerConditionOperator.EQ => ComparerOperator.EQ,
            ComparerConditionOperator.LT => ComparerOperator.LT,
            ComparerConditionOperator.GT => ComparerOperator.GT,
            ComparerConditionOperator.LTE => ComparerOperator.LTE,
            ComparerConditionOperator.GTE => ComparerOperator.GTE,
            ComparerConditionOperator.NEQ => ComparerOperator.NEQ,
            _ => ComparerOperator.EQ
        };
    }

    /// <summary>
    /// Build and return a new ComparerCondition for a certain data type.
    /// </summary>
    /// <param name="comparerDescription">The ConditionMarker describing the condition.</param>
    /// <returns></returns>
    public static Condition BuildComparerCondition(ConditionMarker comparerDescription)
    {
        Condition comparerCondition = null;

        ComparerOperator comparerOperator = ConvertComparerOperator(comparerDescription.comparerOperator);
        string comparerDataKey = comparerDescription.comparerPropertyName;

        switch(comparerDescription.comparerDataType)
        {
            case WorldModelDataType.Integer:
                comparerCondition = new IntComparerCondition(
                comparerDescription.comparerIntValue, comparerDataKey, comparerOperator);
                break;          
            case WorldModelDataType.Float:
                comparerCondition = new FloatComparerCondition(
                comparerDescription.comparerFloatValue, comparerDataKey, comparerOperator);
                break;  
            case WorldModelDataType.Vector2:
                comparerCondition = new Vector2ComparerCondition(
                comparerDescription.comparerVector2Value, comparerDataKey, comparerOperator);
                break;  
            case WorldModelDataType.Vector3:
                comparerCondition = new Vector3ComparerCondition(
                comparerDescription.comparerVector3Value, comparerDataKey, comparerOperator);
                break; 
            case WorldModelDataType.Boolean:
                comparerCondition = new BoolComparerCondition(
                comparerDescription.comparerBoolValue, comparerDataKey, comparerOperator);
                break;  
            case WorldModelDataType.String:
                comparerCondition = new StringComparerCondition(
                comparerDescription.comparerStringValue, comparerDataKey, comparerOperator);
                break;          
        }
        
        return comparerCondition;
    }

    /// <summary>
    /// Build and return a new CompoundCondition aggregating one or more sub-conditions.
    /// </summary>
    /// <param name="compoundDescription"></param>
    /// <param name="subConditions"></param>
    /// <returns></returns>
    public static Condition BuildCompoundCondition(ConditionMarker compoundDescription, Condition[] subConditions)
    {
        Condition compoundCondition = null;

        switch(compoundDescription.compoundOperator)
        {
            case CompoundConditionOperator.AND:
                compoundCondition = new AndCondition(subConditions);
                break;
            case CompoundConditionOperator.OR:
                compoundCondition = new OrCondition(subConditions);
                break;
        }

        return compoundCondition;
    }
}