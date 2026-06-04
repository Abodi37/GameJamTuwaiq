using UnityEngine;

public class CompanionDialogue : MonoBehaviour
{
    public void Talk()
    {
        if (DialogueSystem.Instance == null) return;

        int room = 0;
        int shiftsLeft = 10;

        if (GameManager.Instance != null)
        {
            room = GameManager.Instance.currentRoomIndex;
            shiftsLeft = GameManager.Instance.currentShiftsLeft;
        }

        if (GameManager.Instance != null && GameManager.Instance.HasFlag("exitKeyCollected"))
        {
            DialogueSystem.Instance.StartDialogue(new string[]
            {
                "You have the key.",
                "Do not waste another Shift unless you have to.",
                "The exit is in the hallway."
            });
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.HasFlag("puzzle2Solved"))
        {
            DialogueSystem.Instance.StartDialogue(new string[]
            {
                "The room is waiting for a number now.",
                "Not a code. A count.",
                "Look at your shifts left: " + shiftsLeft + "."
            });
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.HasFlag("puzzle1Solved"))
        {
            DialogueSystem.Instance.StartDialogue(new string[]
            {
                "The cabinet note was not about a normal code.",
                "It was about memory.",
                "What changed the second time we entered?"
            });
            return;
        }

        if (room == 0)
        {
            DialogueSystem.Instance.StartDialogue(new string[]
            {
                "Garage again...",
                "The missing spaces are bait.",
                "Add what is already there."
            });
        }
        else if (room == 1)
        {
            DialogueSystem.Instance.StartDialogue(new string[]
            {
                "This room keeps changing small things.",
                "If you do not remember it now, another Shift will cost you."
            });
        }
        else
        {
            DialogueSystem.Instance.StartDialogue(new string[]
            {
                "Do not stare at the floor here.",
                "If those angels move, I am not saving you."
            });
        }
    }
}