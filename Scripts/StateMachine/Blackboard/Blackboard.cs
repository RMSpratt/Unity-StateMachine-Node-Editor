using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime information for an Agent about the Environment.
/// </summary>
public class Blackboard
{
    Blackboard parent;

    Dictionary<string, int> intEntries;
    Dictionary<string, float> floatEntries;
    Dictionary<string, Vector2> vector2Entries;
    Dictionary<string, Vector3> vector3Entries;
    Dictionary<string, string> stringEntries;
    Dictionary<string, bool> boolEntries;

    public Blackboard(WorldModel worldModel)
    {
        intEntries = worldModel.GetIntEntries();
        floatEntries = worldModel.GetFloatEntries();
        vector2Entries = worldModel.GetVector2Entries();
    }

    /// <summary>
    /// Get the integer value associated with the passed search entry key.
    /// </summary>
    /// <param name="entryKey">The search key as a string value.</param>
    /// <returns></returns>
    public int GetIntValue(string entryKey)
    {
        int entryValue = 0;
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (intEntries.ContainsKey(entryKey))
            {
                entryValue = intEntries[entryKey];
                break;
            }

            currBlackboard = currBlackboard.parent;
        }

        return entryValue;
    }

    /// <summary>
    /// Get the float value associated with the passed search entry key.
    /// </summary>
    /// <param name="entryKey">The search key as a string value.</param>
    /// <returns></returns>
    public float GetFloatValue(string entryKey)
    {
        float entryValue = 0;
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (floatEntries.ContainsKey(entryKey))
            {
                entryValue = floatEntries[entryKey];
                break;
            }

            currBlackboard = currBlackboard.parent;
        }

        return entryValue;
    }

    /// <summary>
    /// Get the Vector2 value associated with the passed search entry key.
    /// </summary>
    /// <param name="entryKey">The search key as a string value.</param>
    /// <returns></returns>
    public Vector2 GetVector2Value(string entryKey)
    {
        Vector2 entryValue = Vector2.zero;
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (vector2Entries.ContainsKey(entryKey))
            {
                entryValue = vector2Entries[entryKey];
                break;
            }

            currBlackboard = currBlackboard.parent;
        }

        return entryValue;
    }

    /// <summary>
    /// Get the Vector3 value associated with the passed search entry key.
    /// </summary>
    /// <param name="entryKey">The search key as a string value.</param>
    /// <returns></returns>
    public Vector3 GetVector3Value(string entryKey)
    {
        Vector3 entryValue = Vector3.zero;
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (vector3Entries.ContainsKey(entryKey))
            {
                entryValue = vector3Entries[entryKey];
                break;
            }

            currBlackboard = currBlackboard.parent;
        }

        return entryValue;
    }

    /// <summary>
    /// Get the Boolean value associated with the passed search entry key.
    /// </summary>
    /// <param name="entryKey">The search key as a string value.</param>
    /// <returns></returns>
    public bool GetBoolValue(string entryKey)
    {
        bool entryValue = false;
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (boolEntries.ContainsKey(entryKey))
            {
                entryValue = boolEntries[entryKey];
                break;
            }

            currBlackboard = currBlackboard.parent;
        }

        return entryValue;
    }

    /// <summary>
    /// Get the string value associated with the passed search entry key.
    /// </summary>
    /// <param name="entryKey">The search key as a string value.</param>
    /// <returns></returns>
    public string GetStringValue(string entryKey)
    {
        string entryValue = "";
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (boolEntries.ContainsKey(entryKey))
            {
                entryValue = stringEntries[entryKey];
                break;
            }

            currBlackboard = currBlackboard.parent;
        }

        return entryValue;
    }

    public void SetParent(Blackboard otherBlackboard)
    {
        if (otherBlackboard != this)
        {
            parent = otherBlackboard;
        }
    }

    /// <summary>
    /// Set the specified Blackboard entry value to the value passed.
    /// </summary>
    /// <param name="entryKey"></param>
    /// <param name="entryValue"></param>
    public void SetIntValue(string entryKey, int entryValue)
    {
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (intEntries.ContainsKey(entryKey))
            {
                intEntries[entryKey] = entryValue;
                break;
            }

            currBlackboard = currBlackboard.parent;
        }
    }

    /// <summary>
    /// Set the specified Blackboard entry value to the value passed.
    /// </summary>
    /// <param name="entryKey"></param>
    /// <param name="entryValue"></param>
    public void SetFloatValue(string entryKey, float entryValue)
    {
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (floatEntries.ContainsKey(entryKey))
            {
                floatEntries[entryKey] = entryValue;
                break;
            }

            currBlackboard = currBlackboard.parent;
        }
    }

    /// <summary>
    /// Set the specified Blackboard entry value to the value passed.
    /// </summary>
    /// <param name="entryKey"></param>
    /// <param name="entryValue"></param>
    public void SetVector2Value(string entryKey, Vector2 entryValue)
    {
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (vector2Entries.ContainsKey(entryKey))
            {
                vector2Entries[entryKey] = entryValue;
                break;
            }

            currBlackboard = currBlackboard.parent;
        }
    }

    /// <summary>
    /// Set the specified Blackboard entry value to the value passed.
    /// </summary>
    /// <param name="entryKey"></param>
    /// <param name="entryValue"></param>
    public void SetVector3Value(string entryKey, Vector3 entryValue)
    {
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (vector3Entries.ContainsKey(entryKey))
            {
                vector3Entries[entryKey] = entryValue;
                break;
            }

            currBlackboard = currBlackboard.parent;
        }
    }

    /// <summary>
    /// Set the specified Blackboard entry value to the value passed.
    /// </summary>
    /// <param name="entryKey"></param>
    /// <param name="entryValue"></param>
    public void SetBoolValue(string entryKey, bool entryValue)
    {
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (boolEntries.ContainsKey(entryKey))
            {
                boolEntries[entryKey] = entryValue;
                break;
            }

            currBlackboard = currBlackboard.parent;
        }
    }

    /// <summary>
    /// Set the specified Blackboard entry value to the value passed.
    /// </summary>
    /// <param name="entryKey"></param>
    /// <param name="entryValue"></param>
    public void SetStringValue(string entryKey, string entryValue)
    {
        Blackboard currBlackboard = this;

        while (currBlackboard != null)
        {
            if (stringEntries.ContainsKey(entryKey))
            {
                stringEntries[entryKey] = entryValue;
                break;
            }

            currBlackboard = currBlackboard.parent;
        }
    }
}
