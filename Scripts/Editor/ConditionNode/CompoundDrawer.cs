using UnityEngine;
using UnityEditor;

/// <summary>
/// /// Singletonn drawing functions for Compound ConditionNodes.
/// </summary>
public static class CompoundDrawer
{
    static readonly string[] compoundOperators = {"AND", "OR"};

    static float halfWidth = (StateMachineEditor.CONDITION_NODE_SIZE[0] - 20) * 0.5f;
    static float labelWidth = (StateMachineEditor.CONDITION_NODE_SIZE[0] - 20) * 0.33f;

    public static void Draw(ref int compoundIdx)
    {
        EditorGUILayout.LabelField(string.Format("{0} Condition", compoundOperators[compoundIdx]), StateMachineEditor.centeredBoldLabelStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("OP:", EditorStyles.boldLabel, GUILayout.Width(labelWidth));
        compoundIdx = EditorGUILayout.Popup(compoundIdx, compoundOperators);
        EditorGUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();

        if ((CompoundConditionOperator)compoundIdx == CompoundConditionOperator.AND)
            EditorGUILayout.LabelField("All subconditions must be met.", StateMachineEditor.italicLabelStyle);

        if ((CompoundConditionOperator)compoundIdx == CompoundConditionOperator.OR)
            EditorGUILayout.LabelField("1+ subconditions must be met.", StateMachineEditor.italicLabelStyle);
    }
}
