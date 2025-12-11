using UnityEngine;

public class ShopController : MonoBehaviour
{
    public ShopItemDefinition[] items;
    public Transform spawnPoint;    // where items appear
    public HUDController hud;       // for demon messages

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip purchaseClip;  

    private bool firstPurchaseDone = false;

    public void TryBuyItem(string itemId)
    {
        var def = System.Array.Find(items, i => i.id == itemId);
        if (def == null)
        {
            Debug.LogWarning("No shop item with id: " + itemId);
            return;
        }

        int coins = GameManager.Instance.Coins;

        if (coins < def.price)
        {
            hud?.ShowError("Maybe come back when you're a bit HMM...RICHER!");
            return;
        }

        GameManager.Instance.AddCoins(-def.price);
        Instantiate(def.itemPrefab, spawnPoint.position, spawnPoint.rotation);

        if (audioSource != null && purchaseClip != null)
        {
            audioSource.PlayOneShot(purchaseClip);
        }

        if (!firstPurchaseDone)
        {
            firstPurchaseDone = true;

            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnFirstShopPurchase();
            }
        }
    }
    
    public void OnShopOpened()
    {
        if (GameManager.Instance.Coins == 0 && items.Length > 0)
        {
            Instantiate(items[0].itemPrefab, spawnPoint.position, spawnPoint.rotation);
            hud?.ShowDemonLine("On the house... don't waste it.");
        }
    }
}