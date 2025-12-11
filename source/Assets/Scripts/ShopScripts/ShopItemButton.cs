using UnityEngine;

public class ShopItemButton : MonoBehaviour
{
    public string itemId;
    public ShopController shop;

    public void OnClick()
    {
        if (shop != null && !string.IsNullOrEmpty(itemId))
        {
            shop.TryBuyItem(itemId);
        }
    }
}