using UnityEngine;

public class PearlPickup : MonoBehaviour
{
    public int value = 1;
    public GameObject collectEffectPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (ScoreManager.instance != null)
            ScoreManager.instance.AddPearl(value);

        if (collectEffectPrefab != null)
            Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

