using UnityEngine;
using UnityEngine.Events;

public class ExitDoorByKey : MonoBehaviour
{
    [Header("Requirement")]
    public string requiredKeyFlag = "exitKeyCollected";

    [Header("Door")]
    public Transform door;
    public Vector3 openRotation = new Vector3(0f, 90f, 0f);
    public float openSpeed = 3f;

    [Header("Events")]
    public UnityEvent onDoorOpened;

    private Quaternion targetRotation;
    private bool opened;

    void Start()
    {
        if (door == null)
            door = transform;

        targetRotation = Quaternion.Euler(door.eulerAngles + openRotation);
    }

    void Update()
    {
        if (opened)
            door.rotation = Quaternion.Lerp(door.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    public void TryOpenDoor()
    {
        if (opened) return;

        if (GameManager.Instance != null && GameManager.Instance.HasFlag(requiredKeyFlag))
        {
            opened = true;
            onDoorOpened.Invoke();

            if (UIManager.Instance != null)
                UIManager.Instance.SetObjective("You escaped.");
        }
        else
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.StartDialogue(new string[]
                {
                    "The exit refuses to move.",
                    "It needs a key from another room."
                });
            }
        }
    }
}