using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serialized representation of a State to be created.
/// </summary>
[System.Serializable]
public class StateMarker
{
    public string stateName;
    public int stateIdx;

    public List<string> entryActions;
    public List<string> regActions;
    public List<string> exitActions;

    public Vector2 graphPosition;
}
