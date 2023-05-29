using UnityEngine;
using UnityEditor;

/// <summary>
/// /// Editor representation of a StateMachine StateTransition using a visual Link.
/// </summary>
public class TransitionLink
{
    SerializedProperty transitionProperty;
    SerializedProperty transitionIdxProperty;
    SerializedProperty transitionNameProperty;

    public StateNode startNode;
    public StateNode endNode;

    public SerializedProperty TransitionProperty {get {return transitionProperty;} private set {}}
    public SerializedProperty TransitionIdxProperty {get {return transitionIdxProperty;} private set {}}
    public SerializedProperty TransitionNameProperty {get {return transitionNameProperty;} private set {}}

    public TransitionLink(StateNode startNode, StateNode endNode, SerializedProperty stateTransition)
    {
        this.startNode = startNode;
        this.endNode = endNode;
        this.transitionProperty = stateTransition;
        transitionIdxProperty = stateTransition.FindPropertyRelative("transitionIdx");
        transitionNameProperty = stateTransition.FindPropertyRelative("transitionName");
    }

    /// <summary>
    /// Editor Draw function to create the TransitionLink.
    /// </summary>
    public void DrawLink()
    {
        Vector2 startPosition = startNode.NodeRect.position + startNode.Midpoint;
        Vector2 endPosition = endNode.NodeRect.position + endNode.Midpoint;
        Vector3 direction = (endPosition - startPosition).normalized;
        Vector3 lineMidpoint;

        Vector2 offsetPosition = new Vector2(direction.y, -direction.x) * 10;
        startPosition += offsetPosition;
        endPosition += offsetPosition;
        lineMidpoint = (startPosition + endPosition) * 0.5f;

        Handles.DrawBezier(
                startPosition,
                endPosition,
                startPosition,
                endPosition,
                Color.white,
                null,
                2f
            );

        //Line isn't smoothly drawn with this method
        //Handles.DrawLine(startNode.NodeRect.center, childNode.NodeRect.center);
        
        float triangleHeight = 15f;
        float triangleWidth = 8;
        Vector3[] trianglePoints = new Vector3[3];
        trianglePoints[1] = lineMidpoint + direction * triangleHeight;
        trianglePoints[0] = lineMidpoint + new Vector3(direction.y, -direction.x) * triangleWidth;
        trianglePoints[2] = lineMidpoint + new Vector3(-direction.y, direction.x) * triangleWidth;
        //Perpendicular vector (dot) direction vector = 0
        Handles.DrawAAConvexPolygon(trianglePoints);
    }
}
