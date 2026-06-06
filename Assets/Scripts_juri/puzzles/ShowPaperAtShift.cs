using UnityEngine;

public class ShowPaperAtShift : MonoBehaviour
{
    [Header("Paper")]
    public GameObject paperObject;

    [Header("Condition")]
    public int requiredRoomIndex = 2;
    public int requiredShiftsLeft = 5;

    void Start()
    {
        UpdatePaperVisibility();
    }

    void Update()
    {
        UpdatePaperVisibility();
    }

    void UpdatePaperVisibility()
    {
        if (paperObject == null) return;

        if (GameManager.Instance == null)
        {
            paperObject.SetActive(false);
            return;
        }

        bool shouldShow =
            GameManager.Instance.currentRoomIndex == requiredRoomIndex &&
            GameManager.Instance.currentShiftsLeft == requiredShiftsLeft;

        paperObject.SetActive(shouldShow);
    }
}