using UnityEngine.Events;

/// <summary>
/// Class information to store multiple MonoBehaviour functions under a unique name moniker.
/// </summary>
[System.Serializable]
public class ActionInfo
{
    public string actionName;
    public UnityEvent actionFuncs;
}
