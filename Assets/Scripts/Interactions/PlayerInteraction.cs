using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Holding")]
    public Transform holdPoint;

    private InteractableObject currentTarget;
    private InteractableObject heldObject;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (holdPoint == null && playerCamera != null)
        {
            GameObject newHoldPoint = new GameObject("HoldPoint");
            newHoldPoint.transform.SetParent(playerCamera.transform);
            newHoldPoint.transform.localPosition = new Vector3(0f, -0.2f, 1.5f);
            newHoldPoint.transform.localRotation = Quaternion.identity;
            holdPoint = newHoldPoint.transform;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (DialogueSystem.Instance != null && DialogueSystem.Instance.IsPlaying)
            return;

        if (heldObject != null)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowInteract("Press E to drop");

            if (Input.GetKeyDown(interactKey))
            {
                heldObject.Drop();
                heldObject = null;

                if (UIManager.Instance != null)
                    UIManager.Instance.ShowInteract("");
            }

            return;
        }

        CheckForInteractable();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact(this);
        }
    }

    void CheckForInteractable()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            InteractableObject target = hit.collider.GetComponentInParent<InteractableObject>();

            if (target != null)
            {
                SetCurrentTarget(target);
                return;
            }
        }

        ClearCurrentTarget();
    }

    void SetCurrentTarget(InteractableObject target)
    {
        if (currentTarget == target) return;

        ClearCurrentTarget();

        currentTarget = target;
        currentTarget.SetHighlighted(true);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteract(currentTarget.GetPrompt());
    }

    void ClearCurrentTarget()
    {
        if (currentTarget != null)
            currentTarget.SetHighlighted(false);

        currentTarget = null;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteract("");
    }

    public Transform GetHoldPoint()
    {
        return holdPoint;
    }

    public void SetHeldObject(InteractableObject obj)
    {
        heldObject = obj;

        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
            currentTarget = null;
        }
    }
}