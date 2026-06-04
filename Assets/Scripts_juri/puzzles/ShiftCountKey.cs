using UnityEngine;
using UnityEngine.Events;

public class ShiftCountKey : MonoBehaviour
{
    [Header("Key Visibility")]
    public GameObject keyVisual;
    public Collider keyCollider;

    [Header("Requirement")]
    public string requiredFlag = "puzzle2Solved";
    public int requiredRoomIndex = 1;
    public int requiredShiftsLeft = 6;

    [Header("Pickup")]
    public string keyFlag = "exitKeyCollected";
    public string inventoryName = "Exit Key";
    public UnityEvent onPickedUp;

    private bool pickedUp;

    void Start()
    {
        SetVisible(false);
    }

    void Update()
    {
        if (pickedUp) return;

        bool canShow = CanShowKey();
        SetVisible(canShow);
    }

    bool CanShowKey()
    {
        if (GameManager.Instance == null) return false;
        if (!GameManager.Instance.HasFlag(requiredFlag)) return false;
        if (GameManager.Instance.currentRoomIndex != requiredRoomIndex) return false;
        if (GameManager.Instance.currentShiftsLeft != requiredShiftsLeft) return false;

        return true;
    }

    public void PickUpKey()
    {
        if (pickedUp) return;

        if (!CanShowKey())
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.StartDialogue(new string[]
                {
                    "Nothing is here yet.",
                    "The key only trusts the correct number of shifts."
                });
            }

            return;
        }

        pickedUp = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetFlag(keyFlag, true);
            GameManager.Instance.AddItem(inventoryName);
        }

        if (UIManager.Instance != null)
            UIManager.Instance.SetObjective("Reach the Angel Hallway and open the exit door.");

        onPickedUp.Invoke();
        SetVisible(false);
    }

    void SetVisible(bool value)
    {
        if (keyVisual != null)
            keyVisual.SetActive(value);

        if (keyCollider != null)
            keyCollider.enabled = value;
    }
}