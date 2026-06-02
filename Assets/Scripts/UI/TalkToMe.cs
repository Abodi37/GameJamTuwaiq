using UnityEngine;

public class TalkToMe : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueSystem dialoguePlayer;

    [TextArea(3, 10)]
    public string[] lines;

    [Header("Interaction")]
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;

    private bool playerInside;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(interactKey))
        {
            if (dialoguePlayer != null)
            {
                dialoguePlayer.StartDialogue(lines);
            }
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