using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Primary EditorWindow for editing StateMachineAgents through a node-based interface.
/// </summary>
public class StateMachineEditor : EditorWindow
{
    //Selected StateMachineAgent properties
    public static GameObject activeAgentObj;
    public static StateMachineAgent activeAgent;
    public static StateMachineGraph activeAgentGraph;
    public static WorldModel activeAgentWorldModel;

    //Serialized Graph data
    public static SerializedObject graphSerializedObj;
    public static SerializedProperty stateListProperty;
    public static SerializedProperty conditionListProperty;
    public static SerializedProperty transitionListProperty;

    static SerializedProperty numGraphStatesProperty;
    static SerializedProperty numGraphTransitionsProperty;
    static SerializedProperty numGraphConditionsProperty;

    //Drawn graph elements
    public static List<StateNode> drawnStateNodes;
    public static List<TransitionLink> drawnTransitionLinks;
    public static List<ConditionNode> drawnConditionNodes;

    //Active drawn elements
    public static StatePanel stateDetailsPanel = null;
    public static LinkPanel linkDetailsPanel = null;


    public static StateNode selectedStateNode = null;
    public static TransitionLink selectedTransitionLink = null;

    public static HashSet<(StateNode, StateNode)> activeLinkPairs;

    //GUIStyle elements for use across the editor
    GUIStyle nodeStyle;
    public static GUIStyle centeredLabelStyle;
    public static GUIStyle centeredBoldLabelStyle;
    public static GUIStyle headerLabelStyle;
    public static GUIStyle subHeaderLabelStyle;
    public static GUIStyle italicLabelStyle;

    GUIStyle detailsPaneStyle;

    //Fixed display properties
    public readonly static Vector2 CONDITION_NODE_SIZE = new Vector2(260, 140);
    public readonly static Vector2 STATE_NODE_SIZE = new Vector2(200,80);

    //Status variables
    public static bool isOpenLinkActive = false;

    Vector2 gridOffset;
    Vector2 dragVector;

    #region GUI Functions

    /// <summary>
    /// Callback function to the EditorWindow being opened.
    /// </summary>
    [MenuItem("StateMachine Editor/Editor")]
    static void ShowEditor()
    {
        StateMachineEditor editor = GetWindow<StateMachineEditor>();
        editor.minSize = new Vector2(800, 600);
    }

    void OnEnable()
    {
        drawnStateNodes = new List<StateNode>();
        drawnTransitionLinks = new List<TransitionLink>();
        activeLinkPairs = new HashSet<(StateNode, StateNode)>();
        drawnConditionNodes = new List<ConditionNode>();
        selectedStateNode = null;
        selectedTransitionLink = null;
        activeAgentGraph = null;
        activeAgent = null;
        activeAgentObj = null;
    
        centeredLabelStyle = new GUIStyle(){alignment = TextAnchor.MiddleCenter};
        centeredLabelStyle.normal.textColor = Color.white;

        centeredBoldLabelStyle = new GUIStyle(){alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold};
        centeredBoldLabelStyle.normal.textColor = Color.white;

        stateDetailsPanel = new StatePanel(OnSelectTransitionLink);
        linkDetailsPanel = new LinkPanel(
            () => {drawnConditionNodes.Clear(); selectedTransitionLink = null;},
            OnDeleteTransitionLink
        );
    }

    //Override the Editor GUI
    void OnGUI()
    {
        GUI.skin.window.padding = new RectOffset(10,10,5,5);
        Event e = Event.current;

        //Defer initialization to OnGUI to ensure EditorStyles is initialized
        if (headerLabelStyle == null)
        {
            headerLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            headerLabelStyle.margin = new RectOffset(0,0,0,(int)EditorGUIUtility.singleLineHeight);
        }

        if (subHeaderLabelStyle == null)
        {
            subHeaderLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            headerLabelStyle.margin = new RectOffset(0,0,0,(int)EditorGUIUtility.standardVerticalSpacing);
        }

        if (italicLabelStyle == null)
        {
            italicLabelStyle = new GUIStyle(EditorStyles.label);
            italicLabelStyle.fontStyle = FontStyle.Italic;
        }

        if (detailsPaneStyle == null)
        {
            // Draw the right sub-window that takes up 30% of the width
            detailsPaneStyle = new GUIStyle(EditorStyles.helpBox);
            detailsPaneStyle.padding = new RectOffset(10, 10, 10, 10);
        }

        if (activeAgentGraph != null && activeAgentGraph != graphSerializedObj.targetObject)
        {
            Debug.Log("Reflect Graph changes");
            graphSerializedObj = new SerializedObject(activeAgentGraph);
        }

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        DrawOpenTransitionLink(e);

        EditorGUILayout.BeginHorizontal();
        GUI.depth = 2;

        if (activeAgentGraph != null)
        {
            if (selectedTransitionLink != null)
            {
                DrawConditionTreeWindow();
            }

            else
            {
                DrawStateMachineWindow();
            }
        }

        GUI.depth = 1;

        DrawDetailsSidePanel();
        EditorGUILayout.EndHorizontal();

        ProcessEvents(e);

        if (graphSerializedObj != null)
        {
            graphSerializedObj.ApplyModifiedProperties();
        }

        if (GUI.changed)
        {
            Repaint();
        }

        if (stateDetailsPanel == null)
            stateDetailsPanel = new StatePanel(OnSelectTransitionLink);
    }
    
    #endregion

    #region Draw Methods

    /// <summary>
    /// Controls drawing the StateMachine as a series of State nodes and Transition links (connections).
    /// </summary>
    void DrawStateMachineWindow()
    {
        GUILayout.BeginArea(new Rect(0, 0, position.width * 0.7f, position.height));
        BeginWindows();

        for (int i = 0; i < drawnStateNodes.Count; i++)
        {
            drawnStateNodes[i].NodeRect = GUI.Window(i, drawnStateNodes[i].NodeRect, DrawStateNode, "");
            EditorGUIUtility.AddCursorRect(drawnStateNodes[i].NodeRect, MouseCursor.MoveArrow);
            drawnStateNodes[i].ProcessEvents(Event.current);
        }

        for (int i = 0; i < drawnTransitionLinks.Count; i++)
        {
            drawnTransitionLinks[i].DrawLink();
        }

        EndWindows();
        GUILayout.EndArea();
    }

    /// <summary>
    /// GUIWindow Draw function for ConditionNodes.
    /// </summary>
    /// <param name="conditionNodeIdx"></param>
    void DrawConditionNode(int conditionNodeIdx)
    {
        drawnConditionNodes[conditionNodeIdx].DrawWindow();
    }

    /// <summary>
    /// GUIWindow Draw function for StateNodes.
    /// </summary>
    /// <param name="stateNodeIdx"></param>
    void DrawStateNode(int stateNodeIdx)
    {
        drawnStateNodes[stateNodeIdx].DrawWindow();
    }

    /// <summary>
    /// Draw the full condition tree for activating the transition.
    /// </summary>
    void DrawConditionTreeWindow()
    {
        GUILayout.BeginArea(new Rect(0, 0, position.width * 0.7f, position.height));
        BeginWindows();

        for (int i = 0; i < drawnConditionNodes.Count; i++)
        {
            drawnConditionNodes[i].NodeRect = GUI.Window(i, drawnConditionNodes[i].NodeRect, DrawConditionNode, "");
            EditorGUIUtility.AddCursorRect(drawnConditionNodes[i].NodeRect, MouseCursor.MoveArrow);
            drawnConditionNodes[i].DrawLinks();
            drawnConditionNodes[i].ProcessEvents(Event.current);
        }

        EndWindows();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draw a side pane describing details of the StateMachine in its entirety.
    /// </summary>
    void DrawDetailsSidePanel()
    {
        GUILayout.BeginArea(new Rect(position.width * 0.7f, 0, position.width * 0.3f, position.height), detailsPaneStyle);

        EditorGUILayout.LabelField("StateMachineAgent Editor", EditorStyles.whiteLargeLabel);
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        GameObject newAgentObj = (GameObject)EditorGUILayout.ObjectField("Agent Graph:", activeAgentObj, typeof(GameObject), false);

        if (newAgentObj != activeAgentObj)
        {
            OnSetStateMachineAgent(newAgentObj);
        }

        if (activeAgentObj != null)
        {
            if (activeAgent != null)
            {
                if (activeAgentGraph != null)
                {
                    EditorGUILayout.PropertyField(graphSerializedObj.FindProperty("agentName"));
                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
    
                    if (selectedTransitionLink != null)
                    {
                        linkDetailsPanel.Draw(selectedTransitionLink.startNode.TargetStateName, selectedTransitionLink.endNode.TargetStateName, activeAgent.GetActionNames());
                    }

                    else if (selectedStateNode != null)
                    {
                        stateDetailsPanel.Draw(activeAgent.GetActionNames(), drawnTransitionLinks);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("The selected StateMachineAgent does not have a StateMachineGraph assigned.", MessageType.Info);
                }
            }

            else
            {
                EditorGUILayout.HelpBox("The selected GameObject does not have a StateMachineAgent MonoBehaviour component.", MessageType.Warning);
            }
        }
   
        else 
        {
            EditorGUILayout.LabelField("Select a StateMachineAgent Graph to begin editing.");
        }
    
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draw an active link being created from the selected state to another state.
    /// </summary>
    /// <param name="e"></param>
    void DrawOpenTransitionLink(Event e)
    {
        if (isOpenLinkActive && selectedStateNode != null)
        {
            Handles.DrawBezier(
                selectedStateNode.NodeRect.center,
                e.mousePosition,
                selectedStateNode.NodeRect.center,
                e.mousePosition,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    /// <summary>
    /// Draw the grid background for viewing nodes and connections.
    /// </summary>
    /// <param name="gridSpacing"></param>
    /// <param name="gridOpacity"></param>
    /// <param name="gridColor"></param>
    // Credit to: https://gram.gs/gramlog/creating-node-based-editor-unity/
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        float gridWidth = position.width * 0.7f;
        //Calculate the number of rows and columns to render
        int widthDivs = Mathf.CeilToInt(gridWidth / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        //Offset the grid using the drag offset
        gridOffset += dragVector * 0.5f;

        Vector3 newOffset = new Vector3(gridOffset.x % gridSpacing, gridOffset.y % gridSpacing, 0);

        //Draw the grid
        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(gridWidth, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    #endregion

    #region Editor Event Methods

    /// <summary>
    /// Add a StateNode to the StateMachineGraph.
    /// </summary>
    /// <param name="arg"></param>
    void OnAddStateNode(object arg)
    {
        Vector2 mousePosition = (Vector2)arg;

        if (stateListProperty != null)
        {
            stateListProperty.InsertArrayElementAtIndex(stateListProperty.arraySize);
            numGraphStatesProperty.intValue += 1;

            SerializedProperty stateProperty = stateListProperty.GetArrayElementAtIndex(stateListProperty.arraySize-1);
            stateProperty.FindPropertyRelative("stateIdx").intValue = numGraphStatesProperty.intValue;
            stateProperty.FindPropertyRelative("stateName").stringValue = string.Format("State_{0}", stateListProperty.arraySize);
            stateProperty.FindPropertyRelative("graphPosition").vector2Value = mousePosition;

            //Clear any values stored in these arrays copied from the previous array element
            stateProperty.FindPropertyRelative("entryActions").ClearArray();
            stateProperty.FindPropertyRelative("regActions").ClearArray();
            stateProperty.FindPropertyRelative("exitActions").ClearArray();

            StateNode nodeToDraw = new StateNode(stateProperty, new Rect(mousePosition.x, mousePosition.y, 200, 80), OnSelectStateNode, OnAddTransitionLink, OnDeleteStateNode);
            drawnStateNodes.Add(nodeToDraw);
        }
    }

    /// <summary>
    /// Add a new ConditionNode to a TransitionLink.
    /// </summary>
    /// <param name="targetTransition">The affected TransitionLink.</param>
    /// <param name="parentCondition">The parent ConditionNode node.</param>
    /// <param name="conditionType">The type of ConditionNode to create.</param>
    void OnAddConditionNode(TransitionLink targetTransition, ConditionNode parentCondition, ConditionMarkerType conditionType)
    {
        SerializedProperty newConditionProperty;
        ConditionNode nodeToDraw;
    
        conditionListProperty.InsertArrayElementAtIndex(conditionListProperty.arraySize);
        numGraphConditionsProperty.intValue += 1;

        newConditionProperty = conditionListProperty.GetArrayElementAtIndex(conditionListProperty.arraySize-1);
        newConditionProperty.FindPropertyRelative("conditionIdx").intValue = numGraphConditionsProperty.intValue;
        newConditionProperty.FindPropertyRelative("transitionIdx").intValue = targetTransition.TransitionIdxProperty.intValue;
        newConditionProperty.FindPropertyRelative("comparerOperator").enumValueIndex = (int)ComparerConditionOperator.EQ;
        newConditionProperty.FindPropertyRelative("comparerDataType").enumValueIndex = (int)WorldModelDataType.Integer;
        newConditionProperty.FindPropertyRelative("comparerPropertyName").stringValue = "";
        newConditionProperty.FindPropertyRelative("comparerIntValue").intValue = 0;
        newConditionProperty.FindPropertyRelative("comparerFloatValue").floatValue = 0;
        newConditionProperty.FindPropertyRelative("comparerStringValue").stringValue = "";
        newConditionProperty.FindPropertyRelative("comparerVector2Value").vector2Value = Vector2.zero;
        newConditionProperty.FindPropertyRelative("comparerVector3Value").vector3Value = Vector3.zero;
        newConditionProperty.FindPropertyRelative("comparerBoolValue").boolValue = false;
        newConditionProperty.FindPropertyRelative("compoundOperator").enumValueIndex = (int)CompoundConditionOperator.AND;

        if (parentCondition != null)
            newConditionProperty.FindPropertyRelative("parentIdx").intValue = parentCondition.ConditionIdx;
        else
            newConditionProperty.FindPropertyRelative("parentIdx").intValue = -1;

        if (conditionType == ConditionMarkerType.Compound)
            newConditionProperty.FindPropertyRelative("conditionType").enumValueIndex = (int)ConditionMarkerType.Compound;
        else
            newConditionProperty.FindPropertyRelative("conditionType").enumValueIndex = (int)ConditionMarkerType.Comparer;


        nodeToDraw = new ConditionNode(newConditionProperty, new Rect(0, 0, 220, 140), OnDeleteConditionNode, OnAddConditionNode);

        graphSerializedObj.ApplyModifiedProperties();
        drawnConditionNodes.Add(nodeToDraw);

        if (parentCondition != null)
        {
            parentCondition.AddSubCondition(nodeToDraw);
        }
    }

    /// <summary>
    /// Add a new TransitionLink connecting the selected StateNode to the target StateNode passed.
    /// </summary>
    /// <param name="targetState"></param>
    void OnAddTransitionLink(StateNode targetState)
    {
        if (!activeLinkPairs.Contains((selectedStateNode, targetState)))
        {
            //Temporary until I think of a way to draw these
            if (selectedStateNode != targetState)
            {
                transitionListProperty.InsertArrayElementAtIndex(transitionListProperty.arraySize);

                SerializedProperty newTransition = transitionListProperty.GetArrayElementAtIndex(transitionListProperty.arraySize-1);
                SerializedProperty newTransitionIdx = newTransition.FindPropertyRelative("transitionIdx");
                SerializedProperty newTransitionName = newTransition.FindPropertyRelative("transitionName");
                SerializedProperty newTransitionStartState = newTransition.FindPropertyRelative("startStateIdx");
                SerializedProperty newTransitionEndState = newTransition.FindPropertyRelative("endStateIdx");

                numGraphTransitionsProperty.intValue += 1;
                newTransitionIdx.intValue = numGraphTransitionsProperty.intValue;
                newTransitionName.stringValue = string.Format("Transition_{0}", 
                    numGraphTransitionsProperty.intValue);

                newTransitionStartState.intValue = selectedStateNode.StateIdxProperty.intValue;
                newTransitionEndState.intValue = targetState.StateIdxProperty.intValue;

                TransitionLink linkToDraw = new TransitionLink(selectedStateNode, targetState, newTransition);
                drawnTransitionLinks.Add(linkToDraw);
                activeLinkPairs.Add((selectedStateNode, targetState));
                OnAddConditionNode(linkToDraw, null, ConditionMarkerType.Compound);
            }
        }

        isOpenLinkActive = false;
    }

    /// <summary>
    /// Set the active transition link for viewing details.
    /// </summary>
    /// <param name="stateLink"></param>
    void OnSelectTransitionLink(TransitionLink stateLink)
    {
        selectedTransitionLink = stateLink;
        drawnConditionNodes.Clear();

        if (stateLink != null)
        {
            linkDetailsPanel.SetActiveLink(stateLink.TransitionProperty);

            //No difference from accessing through the state link, but these values are pre-retrieved
            int startNodeIdx = stateLink.startNode.StateIdxProperty.intValue;
            int endNodeIdx = stateLink.endNode.StateIdxProperty.intValue;

            Dictionary<int, ConditionNode> conditionLookup = new Dictionary<int, ConditionNode>();

            for (int i = 0; i < conditionListProperty.arraySize; i++)
            {
                SerializedProperty conditionProperty = conditionListProperty.GetArrayElementAtIndex(i);
                SerializedProperty conditionTransitionProperty = conditionProperty.FindPropertyRelative("transitionIdx");

                //Only draw conditions related to the current transition
                if (conditionTransitionProperty.intValue == stateLink.TransitionIdxProperty.intValue)
                {
                    SerializedProperty conditionIdxProperty = conditionProperty.FindPropertyRelative("conditionIdx");
                    SerializedProperty conditionParentProperty = conditionProperty.FindPropertyRelative("parentIdx");
                    Vector2 conditionPosition = conditionProperty.FindPropertyRelative("graphPosition").vector2Value;

                    ConditionNode nodeToDraw = new ConditionNode(conditionProperty, new Rect(conditionPosition.x, conditionPosition.y, 220, 140), OnDeleteConditionNode, OnAddConditionNode);
                    drawnConditionNodes.Add(nodeToDraw);
                    conditionLookup.Add(nodeToDraw.ConditionIdx, nodeToDraw);

                    if (conditionLookup.TryGetValue(conditionParentProperty.intValue, out ConditionNode parentCondition))
                    {
                        parentCondition.AddSubCondition(nodeToDraw);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set the active StateNode and its display panel for viewing details.
    /// </summary>
    /// <param name="selectedNode"></param>
    void OnSelectStateNode(StateNode selectedNode)
    {
        if (selectedNode != null && selectedNode != selectedStateNode)
        {
            stateDetailsPanel.SetActiveState(selectedNode.StateProperty);
        }

        selectedStateNode = selectedNode;
    }

    /// <summary>
    /// Set the new StateMachineAgent (if possible) and populate graph details.
    /// </summary>
    /// <param name="newGameObject"></param>
    void OnSetStateMachineAgent(GameObject newGameObject)
    {
        activeAgent = null;
        activeAgentObj = newGameObject;
        activeAgentGraph = null;
        graphSerializedObj = null;
        drawnStateNodes = new List<StateNode>();
        drawnTransitionLinks.Clear();
        drawnConditionNodes.Clear();
        activeLinkPairs.Clear();
        selectedStateNode = null;
        selectedTransitionLink = null;

        if (activeAgentObj != null)
        {
            activeAgent = newGameObject.GetComponent<StateMachineAgent>();

            if (activeAgent != null)
            {
                activeAgentGraph = activeAgent.agentGraph;
            }

            //Display the new agent graph
            if (activeAgentGraph != null)
            {
                activeAgentWorldModel = activeAgentGraph.agentWorldModel;
                ComparerDrawer.OnSetWorldModel();

                graphSerializedObj = new SerializedObject(activeAgentGraph);
                stateListProperty = graphSerializedObj.FindProperty("stateMarkers");
                transitionListProperty = graphSerializedObj.FindProperty("transitionMarkers");
                conditionListProperty = graphSerializedObj.FindProperty("conditionMarkers");
                numGraphStatesProperty = graphSerializedObj.FindProperty("numCreatedStates");
                numGraphTransitionsProperty = graphSerializedObj.FindProperty("numCreatedTransitions");
                numGraphConditionsProperty = graphSerializedObj.FindProperty("numCreatedConditions");

                //2) Create a lookup map for remembering state node position.
                Dictionary<int, StateNode> stateNodeLookup = new Dictionary<int, StateNode>();

                //3) Iterate through the agent states and record the associated name of each.
                for (int i = 0; i < stateListProperty.arraySize; i++)
                {
                    SerializedProperty stateProperty = stateListProperty.GetArrayElementAtIndex(i);
                    Vector2 statePositionProperty = stateProperty.FindPropertyRelative("graphPosition").vector2Value;

                    StateNode nodeToDraw = new StateNode(stateProperty, new Rect(statePositionProperty.x, statePositionProperty.y, 200, 80), OnSelectStateNode, OnAddTransitionLink, OnDeleteStateNode);
                    stateNodeLookup.Add(nodeToDraw.StateIdxProperty.intValue, nodeToDraw);
                    drawnStateNodes.Add(nodeToDraw);
                }

                //4) Iterate through the agent transitions and create links for each
                for (int i = 0; i < transitionListProperty.arraySize; i++)
                {
                    int startStateIdx = transitionListProperty.GetArrayElementAtIndex(i).FindPropertyRelative("startStateIdx").intValue;
                    int endStateIdx = transitionListProperty.GetArrayElementAtIndex(i).FindPropertyRelative("endStateIdx").intValue;

                    TransitionLink linkToDraw = new TransitionLink(stateNodeLookup[startStateIdx], stateNodeLookup[endStateIdx], transitionListProperty.GetArrayElementAtIndex(i));
                    drawnTransitionLinks.Add(linkToDraw);
                    activeLinkPairs.Add((stateNodeLookup[startStateIdx], stateNodeLookup[endStateIdx]));
                }
            }
        }
    }

    /// <summary>
    /// Removes a ConditionNode from the currently selected TransitionLink.
    /// </summary>
    /// <param name="selectedCondition"></param>
    void OnDeleteConditionNode(ConditionNode selectedCondition)
    {
        HashSet<int> conditionPropertyNodesToDelete = new HashSet<int>();
        Queue<ConditionNode> conditionNodesToDelete = new Queue<ConditionNode>();

        conditionNodesToDelete.Enqueue(selectedCondition);

        int lastDeletedIdx = 0;

        while (conditionNodesToDelete.Count > 0)
        {
            ConditionNode nodeToDelete = conditionNodesToDelete.Dequeue();
            int conditionToDeleteIdx = nodeToDelete.ConditionIdx;

            //Check all successor condition nodes that could be a child of the node to delete
            for (int i = lastDeletedIdx; i < drawnConditionNodes.Count; i++)
            {
                SerializedProperty iterConditionProperty = drawnConditionNodes[i].conditionProperty;
                int iterConditionIdx = iterConditionProperty.FindPropertyRelative("conditionIdx").intValue;

                if (drawnConditionNodes[i] == nodeToDelete)
                {
                    //Find the associated ConditionMarker property to delete
                    for (int j = 0; j < conditionListProperty.arraySize; j++)
                    {
                        if (conditionListProperty.GetArrayElementAtIndex(j).FindPropertyRelative("conditionIdx").intValue == iterConditionIdx)
                        {
                            conditionPropertyNodesToDelete.Add(iterConditionIdx);
                            break;
                        }
                    }
                }

                //Keep searching through related nodes
                if (iterConditionProperty.FindPropertyRelative("parentIdx").intValue == conditionToDeleteIdx)
                {
                    conditionNodesToDelete.Enqueue(drawnConditionNodes[i]);
                }
            }
         }

        //Delete the serialized condition noodes found
        for (int k = conditionListProperty.arraySize - 1; k > 0; k--)
        {
            SerializedProperty conditionProperty = conditionListProperty.GetArrayElementAtIndex(k);

            if (conditionPropertyNodesToDelete.Contains(conditionProperty.FindPropertyRelative("conditionIdx").intValue))
            {
                conditionListProperty.DeleteArrayElementAtIndex(k);
            }
        }

        OnSelectTransitionLink(selectedTransitionLink);
    }

    /// <summary>
    /// Deletes an outgoing TransitionLink from the selected StateNode.
    /// </summary>
    /// <param name="linkIndex"></param>
    public void OnDeleteTransitionLink(int linkIndex)
    {
        TransitionLink linkToDelete = drawnTransitionLinks[linkIndex];

        int transitionIdx = linkToDelete.TransitionIdxProperty.intValue;

        //Delete any condition nodes that exist as part of the transition
        for (int i = conditionListProperty.arraySize-1; i >= 0; i--)
        {
            SerializedProperty graphNode = conditionListProperty.GetArrayElementAtIndex(i);

            if (graphNode.FindPropertyRelative("transitionIdx").intValue == transitionIdx)
            {
                conditionListProperty.DeleteArrayElementAtIndex(i);
            }
        }

        for (int j = 0; j < transitionListProperty.arraySize; j++)
        {
            SerializedProperty transitionProperty = transitionListProperty.GetArrayElementAtIndex(j);
            if (transitionProperty.FindPropertyRelative("transitionIdx").intValue == transitionIdx)
            {
                transitionListProperty.DeleteArrayElementAtIndex(j);
                break;
            }
        }

        drawnTransitionLinks.RemoveAt(linkIndex);
        selectedTransitionLink = null;
    }

    /// <summary>
    /// Deletes the currently selected TransitionLink.
    /// </summary>
    /// <param name="linkToDelete">The TransitionLink to be removed.</param>
    public void OnDeleteTransitionLink(TransitionLink linkToDelete)
    {
        if (EditorUtility.DisplayDialog(String.Format("Delete Transition Link: {0}", linkToDelete.TransitionNameProperty.stringValue),
            "Are you sure you want to delete this Transition Link?", "Delete", "Cancel"))
        {
            for (int i = drawnTransitionLinks.Count-1; i >= 0; i--)
            {
                TransitionLink currLink = drawnTransitionLinks[i];

                if (currLink == linkToDelete)
                {
                    OnDeleteTransitionLink(i);
                    break;
                }
            }
        }

        drawnTransitionLinks.Remove(selectedTransitionLink);
        selectedTransitionLink = null;
    }

    /// <summary>
    /// Deletes a StateNode from the StateMachine.
    /// </summary>
    public void OnDeleteStateNode()
    {
        string selectedStateName = selectedStateNode.StateNameProperty.stringValue;
        int stateNodeIdx = selectedStateNode.StateIdxProperty.intValue;

        if (EditorUtility.DisplayDialog(String.Format("Delete State Node: {0}", selectedStateName),
            "Are you sure you want to delete this State Node?", "Delete", "Cancel"))
        {
            HashSet<int> transitionLinksToDelete = new HashSet<int>();

            for (int i = drawnTransitionLinks.Count-1; i >= 0; i--)
            {
                TransitionLink currLink = drawnTransitionLinks[i];

                if (currLink.startNode == selectedStateNode || currLink.endNode == selectedStateNode)
                {
                    OnDeleteTransitionLink(i);
                }
            }

            for (int j = stateListProperty.arraySize-1; j >= 0; j--)
            {
                SerializedProperty stateProperty = stateListProperty.GetArrayElementAtIndex(j);
                if (stateProperty.FindPropertyRelative("stateIdx").intValue == stateNodeIdx)
                {
                    stateListProperty.DeleteArrayElementAtIndex(j);
                    break;
                }
            }

            drawnStateNodes.Remove(selectedStateNode);
            selectedStateNode = null;
        }
    }

    #endregion

    #region User Input Methods

    /// <summary>
    /// Process User input events at the StateMachine level.
    /// </summary>
    /// <param name="e"></param>
    void ProcessEvents(Event e)
    {
        dragVector = Vector2.zero;
        if (e.button == 0)
        {
            if (e.type == EventType.MouseDown)
            {
                LeftClick(e);
            }
        }

        else if (e.button == 1)
        {
            if (e.type == EventType.MouseDown)
            {
                RightClick(e);
            }

        }
    }

    /// <summary>
    /// Process User input left click events.
    /// </summary>
    /// <param name="e"></param>
    void LeftClick(Event e)
    {
        isOpenLinkActive = false;
    }

    /// <summary>
    /// Process User input right click events.
    /// </summary>
    /// <param name="e"></param>
    void RightClick(Event e)
    {
        GenericMenu editorMenu = new GenericMenu();

        if (activeAgentGraph != null)
        {
            if (selectedTransitionLink != null)
            {
                editorMenu.AddItem(new GUIContent("Add Comparer Condition"), false, () => {OnAddConditionNode(selectedTransitionLink, drawnConditionNodes[0], ConditionMarkerType.Comparer);});
                editorMenu.AddItem(new GUIContent("Add Compound Condition"), false, () => {OnAddConditionNode(selectedTransitionLink, drawnConditionNodes[0], ConditionMarkerType.Compound);});
                editorMenu.AddSeparator("");
                editorMenu.AddItem(new GUIContent("Reset Node Positions"), false, () => {
                    drawnConditionNodes.ForEach(conditionNode => {
                        conditionNode.NodeRect = new Rect(0, 0, conditionNode.NodeRect.width, conditionNode.NodeRect.height);
                        });
                });
            }

            else
            {
                editorMenu.AddItem(new GUIContent("Add State"), false, OnAddStateNode, e.mousePosition);
                editorMenu.AddSeparator("");
                editorMenu.AddItem(new GUIContent("Reset Node Positions"), false, () => {
                    drawnStateNodes.ForEach(stateNode => {
                        stateNode.NodeRect = new Rect(0, 0, stateNode.NodeRect.width, stateNode.NodeRect.height);
                        });
                });
            }
        }

        else
        {
            editorMenu.AddDisabledItem(new GUIContent("Add State"));
        }

        editorMenu.ShowAsContext();
    }

    #endregion
}
