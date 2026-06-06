using UnityEngine;

public class CompanionDialogue : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Talk Settings")]
    public float talkDistance = 4f;

    [Header("Auto Banter")]
    public bool autoBanter = true;
    public float minBanterTime = 18f;
    public float maxBanterTime = 35f;

    [Header("General Lines")]
    public string[] generalLines =
    {
        "I do not like how quiet it is.",
        "Stay close. I mean it.",
        "If something changes, remember it.",
        "This place keeps testing us.",
        "Do not waste your shifts."
    };

    [Header("Garage Lines")]
    public string[] garageLines =
    {
        "Garage again...",
        "The missing spaces are bait.",
        "Add what is already there.",
        "That safe is not going to open by guessing."
    };

    [Header("Room 1 Lines")]
    public string[] roomOneLines =
    {
        "This room keeps changing small things.",
        "Look carefully before you shift again.",
        "The answer is probably something you already saw."
    };

    [Header("Room 2 Lines")]
    public string[] roomTwoLines =
    {
        "Do not stare at the floor here.",
        "If those angels move, I am not saving you.",
        "Keep your eyes open."
    };

    private float banterTimer;

    void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        ResetBanterTimer();
    }

    void Update()
    {
        if (!autoBanter) return;
        if (Time.timeScale == 0f) return;

        if (DialogueSystem.Instance != null && DialogueSystem.Instance.IsPlaying)
            return;

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > talkDistance)
            return;

        banterTimer -= Time.deltaTime;

        if (banterTimer <= 0f)
        {
            SayAutoLine();
            ResetBanterTimer();
        }
    }

    public void Talk()
    {
        if (DialogueSystem.Instance == null) return;

        ResetBanterTimer();

        DialogueSystem.Instance.StartDialogue(GetContextDialogue());
    }

    void SayAutoLine()
    {
        if (DialogueSystem.Instance == null) return;

        string line = GetRandomLine(GetContextLines());

        DialogueSystem.Instance.StartDialogue(new string[]
        {
            line
        });
    }

    string[] GetContextDialogue()
    {
        int room = 0;
        int shiftsLeft = 10;

        if (GameManager.Instance != null)
        {
            room = GameManager.Instance.currentRoomIndex;
            shiftsLeft = GameManager.Instance.currentShiftsLeft;
        }

        if (GameManager.Instance != null && GameManager.Instance.HasFlag("exitKeyCollected"))
        {
            return new string[]
            {
                "You have the key.",
                "Do not waste another Shift unless you have to.",
                "The exit is in the hallway."
            };
        }

        if (GameManager.Instance != null && GameManager.Instance.HasFlag("puzzle2Solved"))
        {
            return new string[]
            {
                "The room is waiting for a number now.",
                "Not a code. A count.",
                "Look at your shifts left: " + shiftsLeft + "."
            };
        }

        if (GameManager.Instance != null && GameManager.Instance.HasFlag("puzzle1Solved"))
        {
            return new string[]
            {
                "The safe opened.",
                "Now think about what changed.",
                "What color did you see on the second visit?"
            };
        }

        if (room == 0)
        {
            return new string[]
            {
                "Garage again...",
                "The missing spaces are bait.",
                "Add what is already there."
            };
        }

        if (room == 1)
        {
            return new string[]
            {
                "This room keeps changing small things.",
                "If you do not remember it now, another Shift will cost you."
            };
        }

        return new string[]
        {
            "Do not stare at the floor here.",
            "If those angels move, I am not saving you."
        };
    }

    string[] GetContextLines()
    {
        int room = 0;

        if (GameManager.Instance != null)
            room = GameManager.Instance.currentRoomIndex;

        if (room == 0 && garageLines != null && garageLines.Length > 0)
            return garageLines;

        if (room == 1 && roomOneLines != null && roomOneLines.Length > 0)
            return roomOneLines;

        if (room == 2 && roomTwoLines != null && roomTwoLines.Length > 0)
            return roomTwoLines;

        return generalLines;
    }

    string GetRandomLine(string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return "...";

        int index = Random.Range(0, lines.Length);
        return lines[index];
    }

    void ResetBanterTimer()
    {
        banterTimer = Random.Range(minBanterTime, maxBanterTime);
    }
}