using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FlagInteractable : MonoBehaviour
{
    [Header("Interaction")]
    public string playerTag = "Player";

    [Tooltip("Key used to interact. Read through the new Input System - the project has legacy input disabled.")]
    public Key interactKey = Key.E;

    [Header("Required Flag Optional")]
    public string requiredFlag;
    public bool needRequiredFlag = false;

    [Header("Set Flag After Interaction")]
    public string flagToSet;
    public bool setFlag = true;

    [Header("Dialogue Optional")]
    public DialogueSystem dialoguePlayer;

    [TextArea(2, 8)]
    public string[] successLines;

    [TextArea(2, 8)]
    public string[] failLines;

    [Header("Events")]
    public UnityEvent onSuccess;

    private bool playerInside;
    private bool used;

    void Update()
    {
        if (!playerInside) return;
        if (used) return;

        // The project is set to "Input System Package (New)" only, where
        // UnityEngine.Input.GetKeyDown throws an InvalidOperationException every
        // time it is called.
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        if (keyboard[interactKey].wasPressedThisFrame)
        {
            Interact();
        }
    }

    void Interact()
    {
        if (used) return;

        if (needRequiredFlag)
        {
            if (GameManager.Instance == null) return;

            bool hasFlag = GameManager.Instance.HasFlag(requiredFlag);

            if (!hasFlag)
            {
                if (dialoguePlayer != null && failLines.Length > 0)
                    dialoguePlayer.StartDialogue(failLines);

                return;
            }
        }

        if (setFlag && !string.IsNullOrEmpty(flagToSet) && GameManager.Instance != null)
        {
            GameManager.Instance.SetFlag(flagToSet, true);
        }

        if (dialoguePlayer != null && successLines.Length > 0)
            dialoguePlayer.StartDialogue(successLines);

        onSuccess.Invoke();

        used = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInside = false;
    }
}