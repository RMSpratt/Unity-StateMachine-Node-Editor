using UnityEngine;

/// <summary>
/// The type of ConditionMarker used for Condition creation.
/// </summary>
public enum ConditionMarkerType 
{
    Comparer,
    Compound
}

/// <summary>
/// The comparison operator to be used for evaluating the condition.
/// </summary>
public enum ComparerConditionOperator
{
    EQ,
    LT,
    LTE,
    GT,
    GTE,
    NEQ
}

/// <summary>
/// The compound operator used to aggregate conditions.
/// </summary>
public enum CompoundConditionOperator 
{
    AND,
    OR
}

/// <summary>
/// Serialized representation of a Condition used for StateMachine transitions.
/// </summary>
[System.Serializable]
public class ConditionMarker
{
    public int conditionIdx;
    public int transitionIdx;
    public int parentIdx = -1;   //Sub-conditions can't be stored because it causes a Serialization loop

    public ConditionMarkerType conditionType;

    //Comparer Properties
    public ComparerConditionOperator comparerOperator;
    public string comparerPropertyName;
    public WorldModelDataType comparerDataType;
    public int comparerIntValue = 0;
    public float comparerFloatValue = 0;
    public string comparerStringValue = "";
    public Vector2 comparerVector2Value = Vector2.zero;
    public Vector3 comparerVector3Value = Vector3.zero;
    public bool comparerBoolValue = false;

    //Compound Properties
    public CompoundConditionOperator compoundOperator;

    //Graph properties
    public Vector2 graphPosition;
}
