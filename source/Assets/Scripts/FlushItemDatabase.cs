using UnityEngine;

[System.Serializable]
public class ItemDatabaseEntry
{
    public string id;
    public GameObject prefab;
}

public class FlushItemDatabase : MonoBehaviour
{
    public ItemDatabaseEntry[] items;

    public GameObject GetPrefab(string id)
    {
        foreach (var entry in items)
        {
            if (entry.id == id)
                return entry.prefab;
        }
        return null;
    }
}