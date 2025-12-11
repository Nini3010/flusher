using UnityEngine;

[System.Serializable]
public class ShopItemDefinition
{
    public string id;                 
    public string displayName;        
    public GameObject itemPrefab;     // flushable prefab
    public int price;                 // coins to buy it
    public FlushItemType itemType;    // for info, not strictly required
}