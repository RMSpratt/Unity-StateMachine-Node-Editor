using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

/// <summary>
/// Editor representation of a Node for a Statertansition's condition.
/// </summary>
public class ConditionNode
{
    #region SerializedProperty Information
    public SerializedProperty conditionProperty;
    SerializedProperty conditionPositionProperty;
    SerializedProperty conditionTypeProperty;
    SerializedProperty conditionIdxProperty;

    SerializedProperty comparerDataTypeProperty;
    SerializedProperty comparerKeyNameProperty;
    SerializedProperty comparerKeyValueProperty;
    SerializedProperty comparerOperatorProperty;

    SerializedProperty compoundOperatorProperty;
    #endregion

    #region DrawnNode Properties
    private Rect nodeRect;
    Vector2 prevPosition;
    Vector2 midpoint;

    float halfWidth;

    List<ConditionNode> subconditions = new List<ConditionNode>();

    string[] compoundOperators = {"AND", "OR"};

    //Dropdown indices
    int comparerTypeIdx = 0;
    int oldComparerTypeIdx;

    int comparerOperatorIdx = 0;
    int oldComparerOperatorIdx = 0;

    int compoundOperatorIdx = 0;
    int oldCompoundOperatorIdx;

    int comparerValueIdx = 0;
    int oldComparerValueIdx;

    #endregion

    #region Public Accessors
    public Vector2 Midpoint {get {return midpoint;} set {midpoint = value;}}
    public ConditionMarkerType ConditionType => (ConditionMarkerType)conditionTypeProperty.enumValueIndex;
    public int ConditionIdx => conditionIdxProperty.intValue;
    public Rect NodeRect {get {return nodeRect;} set {nodeRect = value;}}

    #endregion

    #region Node Event Properties
    public bool shouldDeleteNode = false;
    bool isHeld = false;
    UnityAction<ConditionNode> onDelete;
    UnityAction<TransitionLink, ConditionNode, ConditionMarkerType> onAddSubCondition;
    #endregion

    //Constructor
    public ConditionNode(SerializedProperty conditionProperty, Rect nodeRect, UnityAction<ConditionNode> onDelete, UnityAction<TransitionLink, ConditionNode, ConditionMarkerType> onAddSubCondition)
    {
        this.conditionProperty = conditionProperty;
        this.nodeRect = nodeRect;
        this.onDelete = onDelete;
        this.onAddSubCondition = onAddSubCondition;

        conditionIdxProperty = conditionProperty.FindPropertyRelative("conditionIdx");
        conditionPositionProperty = conditionProperty.FindPropertyRelative("graphPosition");
        conditionTypeProperty = conditionProperty.FindPropertyRelative("conditionType");

        prevPosition = nodeRect.position;
        midpoint = new Vector3(nodeRect.width * 0.5f, nodeRect.height * 0.5f);
        halfWidth = (nodeRect.width - 20) * 0.5f;

        //Set Comparer properties
        if (conditionTypeProperty.enumValueIndex == (int)ConditionMarkerType.Comparer)
        {
            comparerDataTypeProperty = conditionProperty.FindPropertyRelative("comparerDataType");
            comparerOperatorProperty = conditionProperty.FindPropertyRelative("comparerOperator");
            comparerKeyNameProperty = conditionProperty.FindPropertyRelative("comparerPropertyName");

            comparerTypeIdx = comparerDataTypeProperty.enumValueIndex;
            oldComparerTypeIdx = comparerTypeIdx;

            comparerOperatorIdx = comparerOperatorProperty.intValue;
            oldComparerOperatorIdx = comparerOperatorIdx;

            comparerValueIdx = ComparerDrawer.GetWorldModelPropertyIndex(
                (WorldModelDataType)comparerDataTypeProperty.enumValueIndex, comparerKeyNameProperty.stringValue);
            oldComparerValueIdx = comparerValueIdx;

            switch (comparerTypeIdx)
            {
                case (int)WorldModelDataType.Integer:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerIntValue");
                    break;
                case (int)WorldModelDataType.Float:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerFloatValue");
                    break;
                case (int)WorldModelDataType.Vector2:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerVector2Value");
                    break;
                case (int)WorldModelDataType.Vector3:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerVector3Value");
                    break;
                case (int)WorldModelDataType.String:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerStringValue");
                    break;
                case (int)WorldModelDataType.Boolean:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerBooleanValue");
                    break;
            }
        }

        else
        {
            compoundOperatorProperty = conditionProperty.FindPropertyRelative("compoundOperator");
            compoundOperatorIdx = compoundOperatorProperty.enumValueIndex;
            oldCompoundOperatorIdx = compoundOperatorIdx;
        }
    }

    /// <summary>
    /// Connect a sub-condition to this ConditionNode
    /// </summary>
    /// <param name="subConditionNode"></param>
    public void AddSubCondition(ConditionNode subConditionNode)
    {
        subconditions.Add(subConditionNode);
    }

    /// <summary>
    /// Draw connections between this node and any child sub-condition nodes.
    /// </summary>
    public void DrawLinks()
    {
        float triangleSize = 10f;

        foreach (ConditionNode childNode in subconditions)
        {
            Vector2 startPosition = nodeRect.position + midpoint;
            Vector2 endPosition = childNode.nodeRect.position + childNode.Midpoint;
            Handles.DrawBezier(
                    startPosition,
                    endPosition,
                    startPosition,
                    endPosition,
                    Color.white,
                    null,
                    2f
                );

            Vector3 lineMidpoint = (startPosition + endPosition) * 0.5f;
            Vector3 direction = (startPosition - endPosition).normalized;
            Vector3[] trianglePoints = new Vector3[3];
            trianglePoints[1] = lineMidpoint + direction * triangleSize;
            trianglePoints[0] = lineMidpoint + new Vector3(direction.y, -direction.x) * triangleSize;
            trianglePoints[2] = lineMidpoint + new Vector3(-direction.y, direction.x) * triangleSize;
            //Perpendicular vector (dot) direction vector = 0
            Handles.DrawAAConvexPolygon(trianglePoints);
        }
    }

    /// <summary>
    /// Draws a Comparer Condition node window and checks for property changes to be saved.
    /// </summary>
    void DrawComparerWindow()
    {
        ComparerDrawer.Draw(ref comparerValueIdx, ref comparerTypeIdx, ref comparerOperatorIdx, comparerKeyValueProperty);

        //Changing DataType requires the operator and property value parameters to be reset
        if (comparerTypeIdx != oldComparerTypeIdx)
        {
            comparerValueIdx = 0;
            comparerOperatorIdx = 0;
            comparerDataTypeProperty.enumValueIndex = comparerTypeIdx;
            oldComparerTypeIdx = comparerTypeIdx;

            switch (comparerTypeIdx)
            {
                case (int)WorldModelDataType.Integer:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerIntValue");
                    break;
                case (int)WorldModelDataType.Float:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerFloatValue");
                    break;
                case (int)WorldModelDataType.Vector2:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerVector2Value");
                    break;
                case (int)WorldModelDataType.Vector3:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerVector3Value");
                    break;
                case (int)WorldModelDataType.String:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerStringValue");
                    break;
                case (int)WorldModelDataType.Boolean:
                    comparerKeyValueProperty = conditionProperty.FindPropertyRelative("comparerBoolValue");
                    break;
            }
        }

        if (comparerOperatorIdx != oldComparerOperatorIdx)
        {
            comparerOperatorProperty.enumValueIndex = comparerOperatorIdx;
        }

        if (oldComparerValueIdx != comparerValueIdx) 
        {
            oldComparerValueIdx = comparerValueIdx;
            comparerKeyNameProperty.stringValue = ComparerDrawer.GetWorldModelPropertyName(comparerTypeIdx, comparerValueIdx);
        }
    }

    /// <summary>
    /// Draws a Compound ConditionNode window and checks for property changes to be saved.
    /// </summary>
    void DrawCompoundWindow()
    {
        CompoundDrawer.Draw(ref compoundOperatorIdx);

        if (compoundOperatorIdx != oldCompoundOperatorIdx)
        {
            compoundOperatorProperty.enumValueIndex = compoundOperatorIdx;
            oldCompoundOperatorIdx = compoundOperatorIdx;
        }
    }

    public void DrawWindow() 
    {
        ConditionMarkerType conditionType = (ConditionMarkerType)conditionTypeProperty.enumValueIndex;

        //Update the graph's saved data
        if (nodeRect.position != prevPosition)
        {
            conditionProperty.FindPropertyRelative("graphPosition").vector2Value = nodeRect.position;
        }

        //Debugging
        //EditorGUILayout.LabelField(string.Format("CIdx: {0} PIdx {1}", this.conditionProperty.FindPropertyRelative("conditionIdx").intValue, this.conditionProperty.FindPropertyRelative("parentIdx").intValue));

        //Draws details for editing the condition as a Comparer
        if (conditionType == ConditionMarkerType.Comparer)
        {
            DrawComparerWindow();
        }

        //Draws details for editing the condition as a collection of sub-conditions
        else if (conditionType == ConditionMarkerType.Compound)
        {
           DrawCompoundWindow();
        }

        prevPosition = nodeRect.position;
    }

    public void DragWindow(Vector2 delta)
    {
        nodeRect.position += delta;
    }

    /// <summary>
    /// Process user input events local to this ConditionNode.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public bool ProcessEvents(Event e)
    {
        if (e.type == EventType.MouseDown && nodeRect.Contains(e.mousePosition))
        {
            if (e.button == 0)
            {
                isHeld = true;
            }

            else if (e.button == 1)
            {
                GenericMenu newConditionMenu = new GenericMenu();

                if (ConditionType == ConditionMarkerType.Compound)
                {
                    newConditionMenu.AddItem(new GUIContent("Add SingleCondition"), false, () => {onAddSubCondition.Invoke(StateMachineEditor.selectedTransitionLink, this, ConditionMarkerType.Comparer);});
                    newConditionMenu.AddItem(new GUIContent("Add Compound Condition"), false, () => {onAddSubCondition.Invoke(StateMachineEditor.selectedTransitionLink, this, ConditionMarkerType.Compound);});
                    newConditionMenu.AddSeparator("");
                }

                if (conditionProperty.FindPropertyRelative("parentIdx").intValue != -1)
                {
                    newConditionMenu.AddItem(new GUIContent("Delete Condition"), false, () => {onDelete(this);});
                }

                else 
                {
                    newConditionMenu.AddDisabledItem(new GUIContent("Delete Condition"));
                }

                newConditionMenu.ShowAsContext();
            }
            e.Use();
        }

        else if (e.type == EventType.MouseUp)
        {
            isHeld = false;
        }

        else if (e.type == EventType.MouseDrag)
        {
            if (e.button == 0 && isHeld)
            {
                DragWindow(e.delta);
                e.Use();
                return true;
            }
        }
        return false;
    }
}
