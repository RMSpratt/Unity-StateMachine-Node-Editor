using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

/// <summary>
/// Panel drawer class for editing the selected StateNode.
/// </summary>
public class StatePanel
{
    //Serialized Data
    SerializedProperty stateNameProperty;
    SerializedProperty stateEntryActionsProperty;
    SerializedProperty stateRegActionsProperty;
    SerializedProperty stateExitActionsProperty;
 
    //StateEditor Properties
    int newEntryActionIdx = 0;
    int newRegActionIdx = 0;
    int newExitActionIdx = 0;

    bool unfoldEntryActions = true;
    bool unfoldRegActions = true;
    bool unfoldExitActions = true;

    //Callback function when the user selects a Transition for editing
    UnityAction<TransitionLink> onSelectTransitionLink;

    public StatePanel(UnityAction<TransitionLink> onSelectTransitionLink)
    {
        this.onSelectTransitionLink = onSelectTransitionLink;
    } 

    /// <summary>
    /// Draw one of the state's action lists for entry, regular, or exit actions.
    /// </summary>
    /// <param name="actionListProperty"></param>
    /// <param name="actionNames"></param>
    /// <param name="actionIdx"></param>
    void DrawActionList(SerializedProperty actionListProperty, string[] actionNames, ref int actionIdx)
    {
        EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.box));

        if (actionListProperty.arraySize > 0)
        {
            for (int i = 0; i < actionListProperty.arraySize; i++)
            {
                bool removeAtIndex = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(actionListProperty.GetArrayElementAtIndex(i).stringValue);
                
                if (GUILayout.Button("Remove"))
                {
                    removeAtIndex = true;
                    actionListProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();

                if (removeAtIndex)
                    break;
            }
        }

        else 
        {
            EditorGUILayout.LabelField("No actions registered.");
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

        EditorGUILayout.BeginHorizontal();
        actionIdx = EditorGUILayout.Popup(actionIdx, actionNames);
        
        if (GUILayout.Button("Add"))
        {
            actionListProperty.InsertArrayElementAtIndex(actionListProperty.arraySize);
            SerializedProperty newStateAction = actionListProperty.GetArrayElementAtIndex(actionListProperty.arraySize-1);
            newStateAction.stringValue = actionNames[actionIdx];
            actionIdx = 0;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
    }

    /// <summary>
    /// Draw the editor panel.
    /// </summary>
    /// <param name="actionNames">A list of actions the agent can carry out.</param>
    /// <param name="outgoingLinks">The outgoing TransitionLinks from this State.</param>
    public void Draw(string[] actionNames, List<TransitionLink> outgoingLinks)
    {
        EditorGUILayout.LabelField((string.Format("Editing State: {0}", stateNameProperty.stringValue)), 
            StateMachineEditor.subHeaderLabelStyle);

        EditorGUILayout.PropertyField(stateNameProperty);
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.BeginScrollView(Vector2.zero);

        //Change property fields to locked read-only lists or dropdown lists(?)
        //Option to add new action from dropdown.
        unfoldEntryActions = EditorGUILayout.Foldout(unfoldEntryActions, "Entry Actions");

        if (unfoldEntryActions)
            DrawActionList(stateEntryActionsProperty, actionNames, ref newEntryActionIdx);

        unfoldRegActions = EditorGUILayout.Foldout(unfoldRegActions, "Regular Actions");

        if (unfoldRegActions)
            DrawActionList(stateRegActionsProperty, actionNames, ref newRegActionIdx);
        
        unfoldExitActions = EditorGUILayout.Foldout(unfoldExitActions, "Exit Actions");

        if (unfoldExitActions)
            DrawActionList(stateExitActionsProperty, actionNames, ref newExitActionIdx);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField("State Transitions", StateMachineEditor.subHeaderLabelStyle);

        if (outgoingLinks != null)
        {
            if (outgoingLinks.Count > 0)
            {
                foreach (TransitionLink outLink in outgoingLinks)
                {
                    if (outLink.startNode == StateMachineEditor.selectedStateNode)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(string.Format("{0}: {1} --> {2}", 
                            outLink.TransitionNameProperty.stringValue,
                            outLink.startNode.StateNameProperty.stringValue,
                            outLink.endNode.StateNameProperty.stringValue));
                        
                        if (GUILayout.Button("Edit Transition"))
                        {
                            onSelectTransitionLink(outLink);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                } 
            }

            else
            {
                EditorGUILayout.LabelField("No transitions created.");
            }
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Sets the selected state details as assigned for the StateMachineEditor.
    /// </summary>
    /// <param name="stateProperty">The SerializedProperty State being edited.</param>
    public void SetActiveState(SerializedProperty stateProperty)
    {
        stateNameProperty = stateProperty.FindPropertyRelative("stateName");
        stateEntryActionsProperty = stateProperty.FindPropertyRelative("entryActions");
        stateRegActionsProperty = stateProperty.FindPropertyRelative("regActions");
        stateExitActionsProperty = stateProperty.FindPropertyRelative("exitActions");
    
        newEntryActionIdx = 0;
        newRegActionIdx = 0;
        newExitActionIdx = 0;
    }
}
