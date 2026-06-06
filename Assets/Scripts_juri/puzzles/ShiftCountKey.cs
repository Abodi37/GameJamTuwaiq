using UnityEngine;

public class ShiftCountKey : MonoBehaviour
{
    [Header("Key Visibility")]
    public GameObject keyVisual;
    public Collider keyCollider;

    [Header("Requirement")]
    public string requiredFlag = "puzzle1Solved";

    [Tooltip("Use -1 to ignore room check.")]
    public int requiredRoomIndex = -1;

    [Tooltip("Use -1 to ignore shift check.")]
    public int requiredShiftsLeft = -1;

    [Header("Pickup")]
    public string keyFlag = "exitKeyCollected";
    public string itemName = "Key";
    public bool destroyAfterPickup = true;

    [Header("Pickup Dialogue")]
    public bool showPickupDialogue = true;
    [TextArea]
    public string pickupLine = "In my pocket you go!";

    private bool pickedUp;

    void Start()
    {
        UpdateVisibility();
    }

    void Update()
    {
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        if (pickedUp)
        {
            SetKeyVisible(false);
            return;
        }

        if (GameManager.Instance == null)
        {
            SetKeyVisible(false);
            return;
        }

        bool hasRequiredFlag = true;

        if (!string.IsNullOrWhiteSpace(requiredFlag))
            hasRequiredFlag = GameManager.Instance.HasFlag(requiredFlag);

        bool roomOk = true;

        if (requiredRoomIndex >= 0)
            roomOk = GameManager.Instance.currentRoomIndex == requiredRoomIndex;

        bool shiftOk = true;

        if (requiredShiftsLeft >= 0)
            shiftOk = GameManager.Instance.currentShiftsLeft == requiredShiftsLeft;

        bool shouldShow = hasRequiredFlag && roomOk && shiftOk;

        SetKeyVisible(shouldShow);
    }

    void SetKeyVisible(bool value)
    {
        if (keyVisual != null)
            keyVisual.SetActive(value);

        if (keyCollider != null)
            keyCollider.enabled = value;
    }

    public void PickUpKey()
    {
        if (pickedUp)
            return;

        pickedUp = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetFlag(keyFlag, true);
            GameManager.Instance.AddItem(itemName);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetKeyIcon(true);
            UIManager.Instance.SetInventoryText("Inventory: Key");
            UIManager.Instance.ShowInteract("");
        }

        if (showPickupDialogue && DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.StartDialogue(new string[]
            {
                pickupLine
            });
        }

        SetKeyVisible(false);

        InteractableObject interactable = GetComponent<InteractableObject>();

        if (interactable != null)
            interactable.enabled = false;

        if (destroyAfterPickup)
            Destroy(gameObject, 0.1f);
    }
}