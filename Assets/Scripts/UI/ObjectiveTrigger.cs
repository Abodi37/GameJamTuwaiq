using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    public string playerTag = "Player";

    [TextArea(2, 5)]
    public string newObjective;

    public bool destroyAfterUse = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (UIManager.Instance != null)
            UIManager.Instance.SetObjective(newObjective);

        if (destroyAfterUse)
            Destroy(gameObject);
    }
}