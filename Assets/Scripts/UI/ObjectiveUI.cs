using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    [TextArea(2, 5)]
    public string startObjective = "Find a way out.";

    void Start()
    {
        SetObjective(startObjective);
    }

    public void SetObjective(string newObjective)
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetObjective(newObjective);
    }

    public void ClearObjective()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetObjective("");
    }
}