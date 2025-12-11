using UnityEngine;

public class StallKnock : MonoBehaviour
{
    [Header("Knock Settings")] 
    public int knocksReqired = 3;
    public float resetTimer = 2f;
    public float minKnockInterval = 0.25f;
    public string handTag = "Hand";
    
    [Header("Shop UI")]
    public GameObject shopMenu;
    
    [Header("Feedback")]
    public AudioSource audioSource;

    public AudioClip[] knockSounds;
    
    private int currentKnocks = 0;
    private float lastKnockTime = 0f;
    private bool storeOpen = false;
    
    void Start()
    {
        if(shopMenu != null)
            shopMenu.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (storeOpen) return;
        if (!collision.collider.CompareTag(handTag)) return;

        Debug.Log("Hit by hand");

        RegisterKnock();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (storeOpen) return;
        if (!other.CompareTag(handTag)) return;
        
        Debug.Log("Door OnTriggerEnter with: " + other.name);
        
        RegisterKnock();
    }
    
    private void RegisterKnock()
    {
        float now = Time.time;

        if (now - lastKnockTime < minKnockInterval)
        {
            return;
        };

        if (now - lastKnockTime > resetTimer)
            currentKnocks = 0;

        lastKnockTime = now;
        currentKnocks++;

        Debug.Log("Valid knock #" + currentKnocks);

        PlayKnockSound();

        if (currentKnocks >= knocksReqired)
            OpenShop();
    }
    private void PlayKnockSound()
    {
        if (audioSource != null && knockSounds.Length > 0)
        {
            int index = Random.Range(0, knockSounds.Length);
            audioSource.PlayOneShot(knockSounds[index]);
        }
    }

    private void OpenShop()
    {
        storeOpen = true;
        currentKnocks = 0;
        
        if (shopMenu != null)
            shopMenu.SetActive(true);
        
        Debug.Log("SHOP OPENED");
        
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnShopDoorOpened();
        }
        
        var shop = shopMenu.GetComponentInChildren<ShopController>();
        if (shop != null)
        {
            shop.OnShopOpened();
        }
    }

    public void CloseShop()
    {
        storeOpen = false;
        if (shopMenu != null)
            shopMenu.SetActive(false);
    }
}