using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    private string[] currentLines;
    private int currentIndex;
    private bool isPlaying;

    public bool IsPlaying
    {
        get { return isPlaying; }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (!isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentIndex = 0;
        isPlaying = true;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowDialogueLine(currentLines[currentIndex]);
    }

    void NextLine()
    {
        currentIndex++;

        if (currentIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowDialogueLine(currentLines[currentIndex]);
    }

    public void EndDialogue()
    {
        isPlaying = false;

        if (UIManager.Instance != null)
            UIManager.Instance.HideDialogue();
    }
}