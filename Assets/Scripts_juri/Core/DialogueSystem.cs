using UnityEngine;
using UnityEngine.InputSystem;

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

    void Start()
    {
        EndDialogue();
    }

    void Update()
    {
        if (!isPlaying)
            return;

        bool spacePressed =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        bool mousePressed =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        if (spacePressed || mousePressed)
            NextLine();
    }

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        currentLines = lines;
        currentIndex = 0;
        isPlaying = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseAllPanelsExcept(UIManager.Instance.dialoguePanel);
            UIManager.Instance.ShowInteract("");
            UIManager.Instance.ShowDialogueLine(currentLines[currentIndex]);
        }
    }

    public void OnContinue(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!isPlaying)
            return;

        NextLine();
    }

    public void NextLine()
    {
        if (!isPlaying)
            return;

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
        currentLines = null;
        currentIndex = 0;

        if (UIManager.Instance != null)
            UIManager.Instance.HideDialogue();
    }
}