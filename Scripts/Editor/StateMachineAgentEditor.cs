using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Editor for an AI Agent implementing a StateMachine.
/// </summary>
[CustomEditor(typeof(StateMachineAgent))]
public class StateMachineAgentEditor : Editor
{
    SerializedObject agent;

    //The SO visual representation of the agent's state machine
    SerializedProperty agentGraphSO;

    void OnEnable()
    {
        agent = new SerializedObject(target);
        agentGraphSO = agent.FindProperty("agentGraph");
    }

    public override void OnInspectorGUI()
    {
        if (agentGraphSO.objectReferenceValue == null)
        {
            if (GUILayout.Button("Create Agent Graph"))
            {
                StateMachineGraph graphToCreate = CreateInstance<StateMachineGraph>();
                graphToCreate.agentName = target.name;
                AssetDatabase.CreateAsset(graphToCreate, string.Format("Assets/ScriptableObjects/{0}.asset", graphToCreate.agentName));
                AssetDatabase.SaveAssets();

                agentGraphSO.objectReferenceValue = graphToCreate;
                agent.ApplyModifiedProperties();
                Repaint();
            }
        }

        else 
        {
            if (GUILayout.Button("Edit Agent Graph"))
            {
                Debug.Log("Open agent graph editor");
            }
        }
        base.OnInspectorGUI();
    }
}
