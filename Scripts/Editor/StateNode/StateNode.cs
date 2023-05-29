using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

/// <summary>
/// Editor representation of a StateMachine state using a visual Node.
/// </summary>
public class StateNode
{

    //Serialized Info for this drawn node's data
    private SerializedProperty stateProperty;
    private SerializedProperty stateIdxProperty;
    private SerializedProperty stateNameProperty;
    private SerializedProperty stateGraphProperty;

    //Info for drawing the node
    Rect nodeRect;
    Vector2 midpoint;
    Vector2 prevPosition;

    //Status information
    bool isHeld = false;

    #region Accessor methods
    public Vector2 Midpoint => midpoint;
    public Rect NodeRect {get {return nodeRect;} set {nodeRect = value;}}
    public string TargetStateName => stateNameProperty.stringValue;
    public SerializedProperty StateProperty {get {return stateProperty;} private set {}}
    public SerializedProperty StateIdxProperty {get {return stateIdxProperty;} private set {}}
    public SerializedProperty StateNameProperty {get {return stateNameProperty;} private set {}}
    
    #endregion

    UnityAction<StateNode> onSelect;
    UnityAction<StateNode> onAddTransition;
    UnityAction onDeleteNode;

    public StateNode(SerializedProperty stateProperty, Rect nodeRect, UnityAction<StateNode> onSelectThis, UnityAction<StateNode> onAddTransition, UnityAction onDeleteNode)
    {
        this.stateProperty = stateProperty;
        this.nodeRect = nodeRect;
        this.onSelect = onSelectThis;
        this.onAddTransition = onAddTransition;
        this.onDeleteNode = onDeleteNode;
        stateIdxProperty = stateProperty.FindPropertyRelative("stateIdx");
        stateNameProperty = stateProperty.FindPropertyRelative("stateName");
        stateGraphProperty = stateProperty.FindPropertyRelative("graphPosition");
        prevPosition = nodeRect.position;
        midpoint = new Vector2(nodeRect.width * 0.5f, nodeRect.height * 0.5f);
    }

    /// <summary>
    /// Editor Draw function to create the StateNode.
    /// </summary>
    public void DrawWindow()
    {
        if (StateMachineEditor.selectedStateNode == this)
        {
            EditorGUILayout.LabelField(stateNameProperty.stringValue, StateMachineEditor.centeredBoldLabelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        else 
        {
            EditorGUILayout.LabelField(stateNameProperty.stringValue, StateMachineEditor.centeredLabelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }
        
        if (nodeRect.position != prevPosition)
        {
            stateGraphProperty.vector2Value = nodeRect.position;
        }

        prevPosition = nodeRect.position;
    }

    /// <summary>
    /// Process user input events affecting this node.
    /// </summary>
    /// <param name="e">User input event</param>
    /// <returns></returns>
    public bool ProcessEvents(Event e)
    {
        if (e.type == EventType.MouseDown && nodeRect.Contains(e.mousePosition))
        {
            if (e.button == 0)
            {
                //Complete the new TransitionLink connection
                if (StateMachineEditor.isOpenLinkActive)
                {
                    onAddTransition.Invoke(this);
                }

                else 
                {
                    isHeld = true;
                    onSelect.Invoke(this);
                }
            }

            else if (e.button == 1)
            {
                onSelect.Invoke(this);
                GenericMenu stateMenu = new GenericMenu();
                stateMenu.AddItem(new GUIContent("Delete State"), false, () => {onDeleteNode.Invoke();});
                stateMenu.AddSeparator("");

                stateMenu.AddItem(new GUIContent("Add Transition"), false, () => {StateMachineEditor.isOpenLinkActive = true;});
                stateMenu.ShowAsContext();
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
                nodeRect.position += e.delta;
                e.Use();
                return true;
            }
        }

        return false;
    }
}
