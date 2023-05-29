using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WorldModelDataType
{
    Integer = 0,
    Float = 1,
    Vector2 = 2,
    Vector3 = 3,
    Boolean = 4,
    String = 5,
}

[CreateAssetMenu]
[System.Serializable]
/// <summary>
/// Design-time representation of knowledge for an Agent.
/// </summary>
public class WorldModel : ScriptableObject
{
    public string modelName;

    private WorldModel oldParent;

    [SerializeField]
    private WorldModel parent;

    [SerializeField]
    private List<WorldDataIntObj> intObjs;

    [SerializeField]
    private List<WorldDataFloatObj> floatObjs;

    [SerializeField]
    private List<WorldDataVector2Obj> vector2Objs;

    [SerializeField]
    private List<WorldDataVector3Obj> vector3Objs;

    [SerializeField]
    private List<WorldDataBoolObj> boolObjs;

    [SerializeField]
    private List<WorldDataStringObj> stringObjs;

    private Dictionary<string, WorldModelDataType> entryLookup;
    private Dictionary<WorldModelDataType, string[]> entryNameLookup;

    //Return a copy of the Dictionary for get access to avoid hot changes breaking Editor functionality
    public Dictionary<WorldModelDataType, string[]> EntryNameLookup {get {return new Dictionary<WorldModelDataType, string[]>(entryNameLookup);} private set {}}

    private void OnEnable() {

        if (intObjs == null)
            intObjs = new List<WorldDataIntObj>();

        if (floatObjs == null)
            floatObjs = new List<WorldDataFloatObj>();

        if (vector2Objs == null)
            vector2Objs = new List<WorldDataVector2Obj>();

        if (vector3Objs == null)
            vector3Objs = new List<WorldDataVector3Obj>();

        if (stringObjs == null)
            stringObjs = new List<WorldDataStringObj>();

        if (boolObjs == null)
            boolObjs = new List<WorldDataBoolObj>();

        entryLookup = new Dictionary<string, WorldModelDataType>();
        entryNameLookup = new Dictionary<WorldModelDataType, string[]>();

        entryNameLookup.Add(WorldModelDataType.Integer, new string[intObjs.Count]);
        entryNameLookup.Add(WorldModelDataType.Float, new string[floatObjs.Count]);
        entryNameLookup.Add(WorldModelDataType.Vector2, new string[vector2Objs.Count]);
        entryNameLookup.Add(WorldModelDataType.Vector3, new string[vector3Objs.Count]);
        entryNameLookup.Add(WorldModelDataType.String, new string[stringObjs.Count]);
        entryNameLookup.Add(WorldModelDataType.Boolean, new string[boolObjs.Count]);

        for (int i = 0; i < intObjs.Count; i++)
        {
            entryLookup.Add(intObjs[i].dataKey, WorldModelDataType.Integer);
            entryNameLookup[WorldModelDataType.Integer][i] = intObjs[i].dataKey;
        }

        for (int i = 0; i < floatObjs.Count; i++)
        {
            entryLookup.Add(floatObjs[i].dataKey, WorldModelDataType.Float);
            entryNameLookup[WorldModelDataType.Float][i] = floatObjs[i].dataKey;
        }
    }

    private void OnValidate() {

        //Re-assign parent keys
        if (parent != oldParent)
        {
            if (parent != null)
            {

            }

            else
            {

            }
            oldParent = parent;
        }
    }

    public bool ContainsEntryKey(string entryKey)
    {
        return entryLookup.ContainsKey(entryKey);
    }

    /// <summary>
    /// Returns true or false indicating if a WorldModelDataType is a numeric type or not.
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public bool IsWorldModelTypeNumeric(WorldModelDataType dataType)
    {
        if (dataType == WorldModelDataType.Integer || dataType == WorldModelDataType.Float || 
            dataType == WorldModelDataType.Vector2 || dataType == WorldModelDataType.Vector3)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true or false indicating if a WorldModelDataType is a numeric type or not.
    /// </summary>
    /// <param name="dataTypeIdx"></param>
    /// <returns></returns>
    public bool IsWorldModelTypeNumeric(int dataTypeIdx)
    {
        return IsWorldModelTypeNumeric((WorldModelDataType)dataTypeIdx);
    }

    /// <summary>
    /// Export a Dictionary copy of the WorldModel's int entries.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, int> GetIntEntries()
    {
        Dictionary<string, int> intEntryLookup = new Dictionary<string, int>();

        foreach (WorldDataIntObj intEntry in intObjs)
        {
            intEntryLookup.Add(intEntry.dataKey, intEntry.dataValue);
        }

        return intEntryLookup;
    }

    /// <summary>
    /// Export a Dictionary copy of the WorldModel's float entries.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, float> GetFloatEntries()
    {
        Dictionary<string, float> floatEntryLookup = new Dictionary<string, float>();

        foreach (WorldDataFloatObj floatEntry in floatObjs)
        {
            floatEntryLookup.Add(floatEntry.dataKey, floatEntry.dataValue);
        }

        return floatEntryLookup;
    }

    /// <summary>
    /// Export a Dictionary copy of the WorldModel's Vector2 entries.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, Vector2> GetVector2Entries()
    {
        Dictionary<string, Vector2> vector2EntryLookup = new Dictionary<string, Vector2>();

        foreach (WorldDataVector2Obj vector2Entry in vector2Objs)
        {
            vector2EntryLookup.Add(vector2Entry.dataKey, vector2Entry.dataValue);
        }

        return vector2EntryLookup;
    }

    /// <summary>
    /// Export a Dictionary copy of the WorldModel's Vector3 entries.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, Vector3> GetVector3Entries()
    {
        Dictionary<string, Vector3> vector3EntryLookup = new Dictionary<string, Vector3>();

        foreach (WorldDataVector3Obj vector3Entry in vector3Objs)
        {
            vector3EntryLookup.Add(vector3Entry.dataKey, vector3Entry.dataValue);
        }

        return vector3EntryLookup;
    }

    /// <summary>
    /// Export a Dictionary copy of the WorldModel's string entries.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> GetStringEntries()
    {
        Dictionary<string, string> stringEntryLookup = new Dictionary<string, string>();

        foreach (WorldDataStringObj stringEntry in stringObjs)
        {
            stringEntryLookup.Add(stringEntry.dataKey, stringEntry.dataValue);
        }

        return stringEntryLookup;
    }

    /// <summary>
    /// Export a Dictionary copy of the WorldModel's boolean entries.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, bool> GetBoolEntries()
    {
        Dictionary<string, bool> boolEntryLookup = new Dictionary<string, bool>();

        foreach (WorldDataBoolObj boolEntry in boolObjs)
        {
            boolEntryLookup.Add(boolEntry.dataKey, boolEntry.dataValue);
        }

        return boolEntryLookup;
    }

    //Hierarchy check for parent keys with the same name
}