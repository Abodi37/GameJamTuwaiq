using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public enum InteractionMode
    {
        Pickup,
        Dialogue,
        EventOnly,
        PickupAndDialogue
    }

    [Header("Highlight")]
    public bool enableHighlight = true;

    [Header("Interaction")]
    public InteractionMode interactionMode = InteractionMode.Pickup;
    public string promptMessage = "Press E";
    public bool oneTimeUse = false;

    [Header("Dialogue Optional")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    [Header("Events Optional")]
    public UnityEvent onInteract;

    [Header("Pickup Settings")]
    public float dropForwardForce = 1.5f;

    [Header("Highlight")]
    public Renderer[] renderers;
    public Color highlightColor = Color.white;
    public float highlightPower = 2f;

    private Rigidbody rb;
    private Collider[] colliders;

    private bool isHeld = false;
    private bool used = false;

    private Material[] materials;
    private Color[] originalEmissionColors;
    private bool[] hadEmissionKeyword;
    private bool materialsCached;
    private SimpleOutline outline;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        outline = GetComponent<SimpleOutline>();

        if (outline == null)
            outline = GetComponentInChildren<SimpleOutline>();

        // Materials are deliberately not touched here. Reading Renderer.materials
        // clones every material and drops the object out of the SRP batcher, so it
        // is deferred until this object is actually highlighted for the first time.
    }

    void CacheMaterials()
    {
        if (materialsCached)
            return;

        materialsCached = true;

        if (renderers == null)
            return;

        int count = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                count += renderers[i].sharedMaterials.Length;
        }

        materials = new Material[count];
        originalEmissionColors = new Color[count];
        hadEmissionKeyword = new bool[count];

        int index = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
                continue;

            Material[] mats = renderers[i].materials;

            for (int j = 0; j < mats.Length && index < materials.Length; j++)
            {
                materials[index] = mats[j];

                if (materials[index].HasProperty("_EmissionColor"))
                {
                    originalEmissionColors[index] = materials[index].GetColor("_EmissionColor");
                    hadEmissionKeyword[index] = materials[index].IsKeywordEnabled("_EMISSION");
                }

                index++;
            }
        }
    }

    public string GetPrompt()
    {
        return promptMessage;
    }

    public void Interact(PlayerInteraction interactor)
    {
        if (oneTimeUse && used) return;

        if (interactionMode == InteractionMode.Dialogue || interactionMode == InteractionMode.PickupAndDialogue)
        {
            if (DialogueSystem.Instance != null && dialogueLines.Length > 0)
            {
                DialogueSystem.Instance.StartDialogue(dialogueLines);
            }
        }

        if (onInteract != null)
            onInteract.Invoke();

        if (interactionMode == InteractionMode.Pickup || interactionMode == InteractionMode.PickupAndDialogue)
        {
            PickUp(interactor);
        }

        used = true;
    }

    void PickUp(PlayerInteraction interactor)
    {
        if (interactor == null) return;
        if (interactor.GetHoldPoint() == null) return;

        isHeld = true;
        SetHighlighted(false);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        transform.SetParent(interactor.GetHoldPoint());
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        interactor.SetHeldObject(this);
    }

    public void Drop()
    {
        if (!isHeld) return;

        Transform oldParent = transform.parent;

        transform.SetParent(null);
        isHeld = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            if (oldParent != null)
                rb.AddForce(oldParent.forward * dropForwardForce, ForceMode.Impulse);
        }
    }

    public void SetHighlighted(bool value)
    {
        if (!enableHighlight)
            value = false;

        if (isHeld)
            value = false;

        if (outline != null)
        {
            outline.SetOutline(value);
            return;
        }

        // Emission fallback. Nothing is cloned until the first actual highlight,
        // so objects that are never looked at keep their shared materials.
        if (!materialsCached)
        {
            if (!value)
                return;

            CacheMaterials();
        }

        if (materials == null) return;

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] == null) continue;
            if (!materials[i].HasProperty("_EmissionColor")) continue;

            if (value)
            {
                materials[i].EnableKeyword("_EMISSION");
                materials[i].SetColor("_EmissionColor", highlightColor * highlightPower);
            }
            else
            {
                materials[i].SetColor("_EmissionColor", originalEmissionColors[i]);

                if (!hadEmissionKeyword[i])
                    materials[i].DisableKeyword("_EMISSION");
            }
        }
    }
}