using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {
    public Image Icon;
    public TextMeshProUGUI Text;
    public TextMeshProUGUI PriceText;
    public Button BuyButton;
    [NonSerialized] public LevelSettings.ShopSetting Item;
    [NonSerialized] public int Price;
    public Action<ShopItem> OnBuy;

    private void Start() {
        BuyButton.onClick.AddListener(() => {
            OnBuy?.Invoke(this);
        });
    }
}