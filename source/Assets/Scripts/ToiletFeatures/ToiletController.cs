using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletController : MonoBehaviour
{
    [Header("Water")]
    public Transform waterPlane;
    public float waterEmptyZ = 0.001558f;
    public float waterOneItemZ = 0.002634f;
    public float waterTwoItemsZ = 0.003577f;
    public float waterLowerDuration = 1.0f;

    [Header("Flush Items")]
    public float itemShrinkDuration = 0.4f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip flushClip;

    [Header("Rules")]
    public int requiredItems = 2;

    [Header("HUD")]
    public HUDController hud;
    public string notEnoughItemsMessage = "You need exactly two items to flush!";

    [Header("Combos")]
    public FlushComboDatabase comboDatabase;

    private readonly List<Rigidbody> itemsInBowl = new List<Rigidbody>();
    private readonly List<FlushableItem> flushItems = new List<FlushableItem>();

    private bool isFlushing;

    // ---------- Item detection ----------
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Flushable")) return;

        var rb = other.attachedRigidbody;
        if (rb != null && !itemsInBowl.Contains(rb))
        {
            itemsInBowl.Add(rb);

            var flushItem = other.GetComponent<FlushableItem>();
            if (flushItem != null && !flushItems.Contains(flushItem))
                flushItems.Add(flushItem);

            UpdateWaterHeight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Flushable")) return;

        var rb = other.attachedRigidbody;
        if (rb != null && itemsInBowl.Remove(rb))
        {
            var flushItem = other.GetComponent<FlushableItem>();
            if (flushItem != null)
                flushItems.Remove(flushItem);

            UpdateWaterHeight();
        }
    }

    private void UpdateWaterHeight()
    {
        int count = Mathf.Clamp(itemsInBowl.Count, 0, 2);

        float targetZ = waterEmptyZ;
        if (count == 1) targetZ = waterOneItemZ;
        else if (count >= 2) targetZ = waterTwoItemsZ;

        Vector3 pos = waterPlane.localPosition;
        pos.z = targetZ;
        waterPlane.localPosition = pos;
    }
    
    public void Flush()
    {
        if (itemsInBowl.Count != requiredItems || flushItems.Count != requiredItems)
        {
            if (hud != null)
                hud.ShowError(notEnoughItemsMessage);

            return;
        }
        
        if (isFlushing) return;
        
        ResolveRewards();
        
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnFirstFlush();
        }
        
        StartCoroutine(FlushRoutine());
    }

    private void ResolveRewards()
    {
        if (flushItems.Count != 2) return;

        var a = flushItems[0];
        var b = flushItems[1];
        if (a == null || b == null) return;

        // Base reward
        int baseReward = a.baseFlushValue + b.baseFlushValue;

        int bonus = 0;

        ComboDefinition combo = comboDatabase?.GetCombo(a.itemType, b.itemType);

        if (combo != null)
        {
            bonus = combo.bonusCoins;

            bool isNew = GameManager.Instance.RegisterCombo(combo);

            if (isNew)
            {
                // Achievement popup
                hud?.ShowAchievement(combo.displayName);

                // Demon says the first-time message
                if (!string.IsNullOrEmpty(combo.firstTimeMessage))
                    hud?.ShowDemonLine(combo.firstTimeMessage);
            }
            else
            {
                // Repeat message (no demon)
                if (!string.IsNullOrEmpty(combo.repeatMessage))
                    hud?.ShowError(combo.repeatMessage);
            }
        }
        else
        {
            // No combo
            hud?.ShowError("Just a regular flush...");
        }

        int total = baseReward + bonus;
        GameManager.Instance.AddCoins(total);
    }


    private IEnumerator FlushRoutine()
    {
        isFlushing = true;

        // Sound
        if (audioSource && flushClip)
            audioSource.PlayOneShot(flushClip);

        // Animate water going down
        Vector3 startPos = waterPlane.localPosition;
        Vector3 endPos = new Vector3(startPos.x, startPos.y, waterEmptyZ - 0.0005f);

        float t = 0f;
        while (t < waterLowerDuration)
        {
            t += Time.deltaTime;
            float k = t / waterLowerDuration;
            waterPlane.localPosition = Vector3.Lerp(startPos, endPos, k);
            yield return null;
        }

        waterPlane.gameObject.SetActive(false);  // briefly empty bowl

        // Shrink + despawn all items
        foreach (var rb in itemsInBowl)
        {
            if (rb == null) continue;
            StartCoroutine(ShrinkAndDestroy(rb.gameObject));
        }
        itemsInBowl.Clear();
        flushItems.Clear();

        // Small delay, then restore idle water
        yield return new WaitForSeconds(0.5f);
        waterPlane.gameObject.SetActive(true);
        Vector3 pos = waterPlane.localPosition;
        pos.z = waterEmptyZ;
        waterPlane.localPosition = pos;

        isFlushing = false;
    }

    private IEnumerator ShrinkAndDestroy(GameObject obj)
    {
        Vector3 startScale = obj.transform.localScale;
        float t = 0f;

        while (t < itemShrinkDuration)
        {
            t += Time.deltaTime;
            float k = t / itemShrinkDuration;
            obj.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, k);
            yield return null;
        }

        Destroy(obj);
    }

    private void Start()
    {
        if (waterPlane != null)
        {
            Vector3 pos = waterPlane.localPosition;
            pos.z = waterEmptyZ;
            waterPlane.localPosition = pos;
        }
    }
}