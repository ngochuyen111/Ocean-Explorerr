using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OxygenSystem : MonoBehaviour
{
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float oxygenDrainPerSecond = 3f;
    public Slider oxygenSlider;

    private PlayerHealth health;

    void Start()
    {
        currentOxygen = maxOxygen;
        health = GetComponent<PlayerHealth>();

        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;
        }
    }

    void Update()
    {
        currentOxygen -= oxygenDrainPerSecond * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

        if (oxygenSlider != null) oxygenSlider.value = currentOxygen;

        if (currentOxygen <= 0)
        {
            if (health != null) health.Die();
            else SceneManager.LoadScene("GameOver");
        }
    }

    public void AddOxygen(float amount)
    {
        currentOxygen = Mathf.Clamp(currentOxygen + amount, 0, maxOxygen);
    }
}