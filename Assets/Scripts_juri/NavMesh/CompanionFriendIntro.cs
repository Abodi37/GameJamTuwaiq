using UnityEngine;

public class CompanionFriendIntro : MonoBehaviour
{
    [Header("References")]
    public HeIsRightBehindMeIsntHe followScript;

    [Header("Dialogue")]
    [TextArea]
    public string[] firstMeetingLines =
    {
        "You accepted the cookies too?",
        "Good. I thought I was alone here.",
        "Stay close. We solve this together."
    };

    [Header("Flags")]
    public string friendFlag = "companionIsFriend";

    [Header("Objective")]
    public string newObjective = "Solve the puzzles together.";

    private bool becameFriend;

    void Start()
    {
        if (followScript == null)
            followScript = GetComponent<HeIsRightBehindMeIsntHe>();

        if (followScript != null)
            followScript.enabled = false;
    }

    public void BecomeFriends()
    {
        if (becameFriend)
            return;

        becameFriend = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseAllPanelsExcept(UIManager.Instance.dialoguePanel);
            UIManager.Instance.ShowInteract("");
            UIManager.Instance.SetObjective(newObjective);
        }

        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.StartDialogue(firstMeetingLines);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.SetFlag(friendFlag, true);

        if (followScript != null)
            followScript.enabled = true;

        InteractableObject interactable = GetComponent<InteractableObject>();

        if (interactable != null)
            interactable.enabled = false;
    }
}