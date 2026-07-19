using UnityEngine;

public class SeaHazard : MonoBehaviour
{
    public float damage = 20f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DamagePlayer(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DamagePlayer(collision);
    }

    private void DamagePlayer(Collision2D collision)
    {
        PlayerHealth health =
            collision.collider.GetComponentInParent<PlayerHealth>();

        if (health == null)
        {
            return;
        }

        health.TakeDamage(damage);
    }
}