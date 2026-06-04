using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public int currentRoomIndex = 0;
    public int currentShiftsLeft = 10;
    public bool gameStarted = false;

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();
    private List<string> inventoryItems = new List<string>();

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
        if (string.IsNullOrEmpty(flagName)) return;

        if (flags.ContainsKey(flagName))
            flags[flagName] = value;
        else
            flags.Add(flagName, value);

        Debug.Log("Flag: " + flagName + " = " + value);
        UpdateInventoryUI();
    }

    public bool HasFlag(string flagName)
    {
        return !string.IsNullOrEmpty(flagName) && flags.ContainsKey(flagName) && flags[flagName];
    }

    public void AddItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return;

        if (!inventoryItems.Contains(itemName))
            inventoryItems.Add(itemName);

        Debug.Log("Item Added: " + itemName);
        UpdateInventoryUI();
    }

    public bool HasItem(string itemName)
    {
        return inventoryItems.Contains(itemName);
    }

    public string GetInventoryText()
    {
        if (inventoryItems.Count == 0)
            return "Inventory: Empty";

        return "Inventory: " + string.Join(", ", inventoryItems);
    }

    public void SetCurrentRoom(int roomIndex)
    {
        currentRoomIndex = roomIndex;
    }

    public void SetShiftsLeft(int shiftsLeft)
    {
        currentShiftsLeft = shiftsLeft;
    }

    public void UpdateInventoryUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetInventoryText(GetInventoryText());
            UIManager.Instance.SetCookieIcon(HasFlag("hasCookies"));
            UIManager.Instance.SetKeyIcon(HasFlag("exitKeyCollected"));
        }
    }
}