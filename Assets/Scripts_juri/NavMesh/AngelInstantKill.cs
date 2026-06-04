using UnityEngine;

public class AngelInstantKill : MonoBehaviour
{
    public string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();

        if (stats != null)
            stats.Die();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag)) return;

        PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();

        if (stats != null)
            stats.Die();
    }
}