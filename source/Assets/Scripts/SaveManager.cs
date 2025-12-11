using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    private const string SAVE_KEY = "ToiletGameSave";

    [Header("Links")]
    public HUDController hud;
    public Transform recoveryTable;
    public float spacing = 0.25f;

    public FlushItemDatabase itemDB;

    // ---------- PUBLIC API: CALLED BY UI BUTTONS ----------
    public void SaveGame()
    {
        SaveData data = new SaveData();

        // coins
        data.coins = GameManager.Instance.Coins;

        // unlocked combos / achievements
        data.unlockedCombos = GameManager.Instance.GetUnlockedComboIds();

        // inventory items (items physically in scene)
        Dictionary<string, int> counts = new Dictionary<string, int>();
        
        data.tutorialCompleted = TutorialManager.Instance != null &&
                                 TutorialManager.Instance.WasTutorialCompleted();

        foreach (var f in FindObjectsOfType<FlushableItem>())
        {
            if (!f.saveToInventory) continue;
            if (string.IsNullOrEmpty(f.itemId)) continue;

            if (!counts.ContainsKey(f.itemId))
                counts[f.itemId] = 0;

            counts[f.itemId]++;
        }

        foreach (var kv in counts)
        {
            data.inventory.Add(new SavedItem { id = kv.Key, count = kv.Value });
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        hud.ShowDemonLine("Your worldly possessions are… preserved.");
    }


    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            hud.ShowDemonLine("No saved offerings found.");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // restore coins
        GameManager.Instance.SetCoins(data.coins);

        // restore achievements
        GameManager.Instance.SetUnlockedCombos(data.unlockedCombos);

        // clear old items from table
        ClearRecoveryTable();
        
        TutorialManager.Instance?.SetTutorialCompleted(data.tutorialCompleted);

        // respawn saved items on table
        int index = 0;
        foreach (var entry in data.inventory)
        {
            GameObject prefab = itemDB.GetPrefab(entry.id);
            if (prefab == null)
            {
                Debug.LogWarning("No prefab for saved id " + entry.id);
                continue;
            }

            for (int i = 0; i < entry.count; i++)
            {
                Vector3 pos = recoveryTable.position +
                              new Vector3((index % 5) * spacing, 0, (index / 5) * spacing);

                Instantiate(prefab, pos, recoveryTable.rotation);
                index++;
            }
        }

        hud.ShowDemonLine("Your lost treasures… restored.");
    }


    // ---------- item recovery button ----------
    public void RecoverItems()
    {
        ClearRecoveryTable();

        var flushables = FindObjectsOfType<FlushableItem>();
        int index = 0;

        foreach (var f in flushables)
        {
            if (!f.saveToInventory) continue;

            Vector3 pos = recoveryTable.position +
                          new Vector3((index % 5) * spacing, 0, (index / 5) * spacing);

            f.transform.position = pos;
            f.transform.rotation = recoveryTable.rotation;

            index++;
        }

        hud.ShowDemonLine("Anything lost… now returns to the table.");
    }


    private void ClearRecoveryTable()
    {
        var flushables = FindObjectsOfType<FlushableItem>();
        foreach (var f in flushables)
        {
            // Destroy only items sitting on the table or belonging to inventory
            if (f.saveToInventory)
                Destroy(f.gameObject);
        }
    }
}