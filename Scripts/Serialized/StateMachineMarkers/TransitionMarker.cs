using System.Collections.Generic;

/// <summary>
/// Serialized representation of a StateTransition to be created.
/// </summary>
[System.Serializable]
public class TransitionMarker
{
    public int transitionIdx;
    public string transitionName;

    //The States that this Transition is tied to
    public int startStateIdx;
    public int endStateIdx;

    public List<string> transitionActions;
}
