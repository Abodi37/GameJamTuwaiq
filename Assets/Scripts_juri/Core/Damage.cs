using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage = 10;
    public float damageCooldown = 1f;
    public string playerTag = "Player";

    private float lastDamageTime;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (Time.time >= lastDamageTime + damageCooldown)
        {
            HealthSystem health = other.GetComponent<HealthSystem>();

            if (health != null)
            {
                health.TakeDamage(damage);
                lastDamageTime = Time.time;
            }
        }
    }
}