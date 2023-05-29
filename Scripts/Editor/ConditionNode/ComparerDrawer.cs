using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Singletonn drawing functions for Comparer ConditionNodes.
/// </summary>
public static class ComparerDrawer
{
    static Dictionary<WorldModelDataType, string[]> comparerValueOptions;

    static readonly string[] dataTypes = {"Integer", "Float", "Vector2", "Vector3", "Boolean", "String"};
    static readonly string[] numericOperators = {"EQ (==)", "LT (<)", "LTE (<=)", "GT (>)", "GTE (>=)", "NEQ (!=)"};
    static readonly string[] nonNumericOperators = {"EQ (==)", "NEQ (!=)"};

    static float labelWidth = (StateMachineEditor.CONDITION_NODE_SIZE[0] - 20) * 0.33f;

    public static void OnSetWorldModel()
    {
        comparerValueOptions = StateMachineEditor.activeAgentWorldModel.EntryNameLookup;
    }

    /// <summary>
    /// Returns the name of a WorldModel property using its type and index.
    /// </summary>
    /// <param name="propertyTypeIdx"></param>
    /// <param name="valueIdx"></param>
    /// <returns></returns>
    public static string GetWorldModelPropertyName(int propertyTypeIdx, int valueIdx)
    {
        string propertyName = "";

        if (valueIdx < comparerValueOptions[(WorldModelDataType)propertyTypeIdx].Length)
        {
            propertyName = comparerValueOptions[(WorldModelDataType)propertyTypeIdx][valueIdx];
        }

        return propertyName;
    }

    /// <summary>
    /// Looksup a WorldModel property by name and returns the matching index.
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static int GetWorldModelPropertyIndex(WorldModelDataType propertyType, string propertyName)
    {
        int propertyIdx = -1;

        for (int i = 0; i < comparerValueOptions[propertyType].Length; i++)
        {
            if (comparerValueOptions[propertyType][i] == propertyName)
            {
                propertyIdx = i;
                break;
            }
        }

        return propertyIdx;
    }

    /// <summary>
    /// Draw the Comparer Condition Node.
    /// </summary>
    /// <param name="valueIdx">The selected WorldModel property.</param>
    /// <param name="typeIdx">The selected WorldModel property data type. </param>
    /// <param name="opIdx">The selected comparison operator.</param>
    public static void Draw(ref int valueIdx, ref int propertyTypeIdx, ref int opIdx, SerializedProperty valueProperty)
    {
        string[] valueOptions = {};
        string[] operatorOptions = {};

        int displayValueIdx = valueIdx;

        valueOptions = comparerValueOptions[(WorldModelDataType)propertyTypeIdx];
        
        if (StateMachineEditor.activeAgentWorldModel.IsWorldModelTypeNumeric(propertyTypeIdx))
            operatorOptions = numericOperators;
        else
            operatorOptions = nonNumericOperators;

        //The condition is referencing an invalid property.
        if (displayValueIdx == -1)
        {
            EditorGUILayout.HelpBox("Unrecognized condition value. Please select a new one.", MessageType.Warning);
            displayValueIdx = 0;
        }

        EditorGUILayout.LabelField("Comparer Condition", StateMachineEditor.centeredBoldLabelStyle);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Type:", EditorStyles.boldLabel, GUILayout.Width(labelWidth));
        propertyTypeIdx = EditorGUILayout.Popup(propertyTypeIdx, dataTypes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Var:", EditorStyles.boldLabel, GUILayout.Width(labelWidth));
        displayValueIdx = EditorGUILayout.Popup(displayValueIdx, valueOptions);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("OP:", EditorStyles.boldLabel, GUILayout.Width(labelWidth));
        opIdx = EditorGUILayout.Popup(opIdx, operatorOptions);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Value:", EditorStyles.boldLabel, GUILayout.Width(labelWidth));
        EditorGUILayout.PropertyField(valueProperty, GUIContent.none, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        if (displayValueIdx != valueIdx)
            valueIdx = displayValueIdx;
    }
}
