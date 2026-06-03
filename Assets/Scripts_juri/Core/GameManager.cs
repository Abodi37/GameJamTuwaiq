using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetFlag(string flagName, bool value)
    {
        if (flags.ContainsKey(flagName))
            flags[flagName] = value;
        else
            flags.Add(flagName, value);

        Debug.Log("Flag changed: " + flagName + " = " + value);
    }

    public bool HasFlag(string flagName)
    {
        return flags.ContainsKey(flagName) && flags[flagName];
    }
}