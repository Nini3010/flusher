using UnityEngine;

public enum FlushItemType
{
    Can,
    Soap,
    Cigarettes,
    HotSauce
    
}
public class FlushableItem : MonoBehaviour
{
    public FlushItemType itemType;
    public int baseFlushValue = 1;

    [Header("Saving")]
    public string itemId;           // must match ShopItemDefinition.id and DB id
    public bool saveToInventory = true;
}