using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel;

    private PlayerHealth playerHealth;
    private OxygenSystem oxygenSystem;
    private PlayerController playerController;
    private bool isShopOpen = false;
    public GameObject openShopButton;
    public TextMeshProUGUI shopMessageText;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            oxygenSystem = player.GetComponent<OxygenSystem>();
            playerController = player.GetComponent<PlayerController>();
        }

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void OpenShop()
    {
        isShopOpen = true;
        shopPanel.SetActive(true);
        shopMessageText.text = "";

        if (openShopButton != null)
            openShopButton.SetActive(false);

        Time.timeScale = 0f;
    }
    public void CloseShop()
    {
        isShopOpen = false;
        shopPanel.SetActive(false);

        if (openShopButton != null)
            openShopButton.SetActive(true);

        Time.timeScale = 1f;
    }

    public void BuyOxygen()
    {
        if (ScoreManager.instance.pearls < 10)
        {
            shopMessageText.text = "Not enough pearls!";
            return;
        }

        shopMessageText.text = "";

        if (oxygenSystem != null)
        {
            oxygenSystem.BuyOxygen();
        }
    }

    public void BuyHealth()
    {
        if (ScoreManager.instance.pearls < 15)
        {
            shopMessageText.text = "Not enough pearls!";
            return;
        }

        shopMessageText.text = "";

        if (playerHealth != null)
        {
            playerHealth.BuyHealth();
        }
    }

    public void BuySpeed()
    {
        if (ScoreManager.instance.pearls < 20)
        {
            shopMessageText.text = "Not enough pearls!";
            return;
        }

        shopMessageText.text = "";

        if (playerController != null)
        {
            playerController.BuySpeed();
        }
    }
}