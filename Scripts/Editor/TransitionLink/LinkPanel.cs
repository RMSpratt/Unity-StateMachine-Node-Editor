using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

/// <summary>
/// Panel drawer class for editing the selected TransitionLink.
/// </summary>
public class LinkPanel
{
    //Serialized Data
    SerializedProperty transitionNameProperty;
    SerializedProperty transitionActionsProperty;

    int transitionActionIdx;

    UnityAction<TransitionLink> onDeleteTransitionLink;
    UnityAction onDeselectTransitionLink;

    public LinkPanel(UnityAction onExitLinkDetails, UnityAction<TransitionLink> onDeleteTransition)
    {
        this.onDeselectTransitionLink = onExitLinkDetails;
        this.onDeleteTransitionLink = onDeleteTransition;
    }

    /// <summary>
    /// Draw the editor panel.
    /// </summary>
    /// <param name="startStateName"></param>
    /// <param name="endStateName"></param>
    /// <param name="agentActions"></param>
    public void Draw(string startStateName, string endStateName, string[] agentActions)
    {
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        EditorGUILayout.LabelField((string.Format("Editing Transition: {0} - {1}",
            startStateName, endStateName)),  StateMachineEditor.subHeaderLabelStyle);
        EditorGUILayout.PropertyField(transitionNameProperty);
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

        EditorGUILayout.LabelField("Transition Actions", StateMachineEditor.subHeaderLabelStyle);
        EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.box));

        if (transitionActionsProperty.arraySize > 0)
        {
            for (int i = 0; i < transitionActionsProperty.arraySize; i++)
            {
                bool removeAtIndex = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(transitionActionsProperty.GetArrayElementAtIndex(i).stringValue);

                if (GUILayout.Button("Remove"))
                {
                    removeAtIndex = true;
                    transitionActionsProperty.DeleteArrayElementAtIndex(i);
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
        transitionActionIdx = EditorGUILayout.Popup(transitionActionIdx, agentActions);

        if (GUILayout.Button("Add"))
        {
            transitionActionsProperty.InsertArrayElementAtIndex(transitionActionsProperty.arraySize);
            SerializedProperty newStateAction = transitionActionsProperty.GetArrayElementAtIndex(transitionActionsProperty.arraySize-1);
            newStateAction.stringValue = agentActions[transitionActionIdx];
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete Transition"))
        {
            onDeleteTransitionLink.Invoke(StateMachineEditor.selectedTransitionLink);
        }
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

        if (GUILayout.Button("Back to State"))
        {
            onDeselectTransitionLink.Invoke();
        }
    }

    /// <summary>
    /// Sets the selected TransitionLink's details as assigned for the StateMachineEditor.
    /// </summary>
    /// <param name="linkProperty">The SerializedProperty StateTransition being edited.</param>
    public void SetActiveLink(SerializedProperty linkProperty)
    {
        transitionNameProperty = linkProperty.FindPropertyRelative("transitionName");
        transitionActionsProperty = linkProperty.FindPropertyRelative("transitionActions");
        transitionActionIdx = 0;
    }
}
