using UnityEngine;

public class ColorMemoryObject : MonoBehaviour
{
    [Header("Room")]
    public int roomIndex = 1;

    [Header("Object Renderer")]
    public Renderer targetRenderer;

    [Header("Visit Colors")]
    public Color[] visitColors = { Color.red, Color.blue, Color.green, Color.yellow };
    public string[] colorNames = { "Red", "Blue", "Green", "Yellow" };

    private int lastVisit = -1;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (RoomManager.Instance == null) return;

        int visit = RoomManager.Instance.GetRoomVisitCount(roomIndex);

        if (visit != lastVisit)
        {
            lastVisit = visit;
            ApplyVisitColor(visit);
        }
    }

    void ApplyVisitColor(int visitNumber)
    {
        if (targetRenderer == null) return;
        if (visitColors == null || visitColors.Length == 0) return;

        int index = Mathf.Clamp(visitNumber - 1, 0, visitColors.Length - 1);
        targetRenderer.material.color = visitColors[index];
    }

    public string GetColorNameForVisit(int visitNumber)
    {
        if (colorNames == null || colorNames.Length == 0)
            return "";

        int index = Mathf.Clamp(visitNumber - 1, 0, colorNames.Length - 1);
        return colorNames[index];
    }
}