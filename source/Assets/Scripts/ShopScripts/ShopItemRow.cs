using UnityEngine;
using TMPro;

public class ShopItemRow : MonoBehaviour
{
    public TextMeshProUGUI labelText;
    public ShopItemButton buyButton;

    public void Init(ShopItemDefinition def, ShopController shop)
    {
        if (labelText != null)
        {
            labelText.text = $"{def.displayName} - {def.price} YEN";
        }

        if (buyButton != null)
        {
            buyButton.itemId = def.id;
            buyButton.shop = shop;
        }
    }
}