using UnityEngine;
using UnityEngine.InputSystem;

public class TalkToMe : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueSystem dialoguePlayer;

    [TextArea(3, 10)]
    public string[] lines;

    [Header("Interaction")]
    public string playerTag = "Player";

    [Tooltip("Key used to talk. Read through the new Input System - the project has legacy input disabled.")]
    public Key interactKey = Key.E;

    private bool playerInside;

    void Update()
    {
        if (!playerInside)
            return;

        // The project is set to "Input System Package (New)" only, where
        // UnityEngine.Input.GetKeyDown throws an InvalidOperationException every
        // time it is called.
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        if (!keyboard[interactKey].wasPressedThisFrame)
            return;

        if (dialoguePlayer != null)
        {
            dialoguePlayer.StartDialogue(lines);
        }
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