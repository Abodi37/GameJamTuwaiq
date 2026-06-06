using UnityEngine;

public class SimpleOutline : MonoBehaviour
{
    [Header("Outline")]
    public Color outlineColor = Color.white;
    public float outlineScale = 1.03f;

    private GameObject outlineObject;
    private Renderer[] outlineRenderers;

    void Awake()
    {
        CreateOutline();
        SetOutline(false);
    }

    void CreateOutline()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one;

        Material outlineMaterial = new Material(Shader.Find("Unlit/Color"));
        outlineMaterial.color = outlineColor;

        for (int i = 0; i < meshFilters.Length; i++)
        {
            MeshFilter sourceFilter = meshFilters[i];
            MeshRenderer sourceRenderer = sourceFilter.GetComponent<MeshRenderer>();

            if (sourceRenderer == null)
                continue;

            GameObject child = new GameObject(sourceFilter.gameObject.name + "_Outline");
            child.transform.SetParent(outlineObject.transform);

            child.transform.position = sourceFilter.transform.position;
            child.transform.rotation = sourceFilter.transform.rotation;
            child.transform.localScale = sourceFilter.transform.lossyScale * outlineScale;

            MeshFilter newFilter = child.AddComponent<MeshFilter>();
            newFilter.sharedMesh = sourceFilter.sharedMesh;

            MeshRenderer newRenderer = child.AddComponent<MeshRenderer>();
            newRenderer.sharedMaterial = outlineMaterial;

            child.layer = sourceFilter.gameObject.layer;
        }

        outlineRenderers = outlineObject.GetComponentsInChildren<Renderer>();
    }

    public void SetOutline(bool value)
    {
        if (outlineObject != null)
            outlineObject.SetActive(value);
    }
}