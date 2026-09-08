using UnityEngine;

public class SimpleOutline : MonoBehaviour
{
    [Header("Outline")]
    public Color outlineColor = Color.white;
    public float outlineScale = 1.03f;

    // One material shared by every outline in the scene, so the SRP batcher
    // can still batch the outline hulls together.
    private static Material sharedOutlineMaterial;
    private static bool shaderLookupFailed;

    private GameObject outlineObject;
    private bool built;

    public void SetOutline(bool value)
    {
        // Built on first use instead of in Awake: an object that is never
        // highlighted never pays for the duplicated meshes.
        if (value && !built)
            CreateOutline();

        if (outlineObject != null)
            outlineObject.SetActive(value);
    }

    void CreateOutline()
    {
        built = true;

        Material outlineMaterial = GetOutlineMaterial();

        if (outlineMaterial == null)
            return;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        if (meshFilters.Length == 0)
            return;

        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one;

        for (int i = 0; i < meshFilters.Length; i++)
        {
            MeshFilter sourceFilter = meshFilters[i];

            if (sourceFilter.sharedMesh == null)
                continue;

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
            newRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            newRenderer.receiveShadows = false;
            newRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            newRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            child.layer = sourceFilter.gameObject.layer;
        }

        outlineObject.SetActive(false);
    }

    Material GetOutlineMaterial()
    {
        if (sharedOutlineMaterial != null)
            return sharedOutlineMaterial;

        if (shaderLookupFailed)
            return null;

        // "Unlit/Color" is a Built-in pipeline shader and renders magenta under
        // URP, so look for the URP shader first. Shader.Find only sees shaders
        // that shipped with the build, which is true for both of these.
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            shaderLookupFailed = true;
            Debug.LogWarning("SimpleOutline: no usable outline shader found, outlines are disabled.");
            return null;
        }

        sharedOutlineMaterial = new Material(shader);
        sharedOutlineMaterial.color = outlineColor;

        return sharedOutlineMaterial;
    }
}
