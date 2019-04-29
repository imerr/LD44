using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
    public Button RerollButton;
    public TextMeshProUGUI RerollPriceText;
    public Button CloseButton;
    public ShopItem[] Items;
    private LevelManager _manager;

    private void Start() {
        CloseButton.onClick.AddListener(Close);
        RerollButton.onClick.AddListener(() => {
            RemoveMoney((int)_manager.Settings.RerollCost.Evaluate(LevelManager.CurrentLevel));
            Reroll();
        });
        RerollPriceText.text = ((int) _manager.Settings.RerollCost.Evaluate(LevelManager.CurrentLevel)).ToString();
        foreach (var item in Items) {
            item.OnBuy += OnBuy;
        }
    }

    private void RemoveMoney(int amount) {
        if (!Player.Instance) {
            return;
        }
        Player.Instance.ReceiveDamage(amount, null);
        if (Player.Instance.Health <= 0) {
            Close();
        }
    }

    private void OnBuy(ShopItem item) {
        if (!Player.Instance) {
            return;
        }
        RemoveMoney(item.Price);
        Player.Instance.GetComponent<WeaponPositioner>()?.Add(item.Item.Prefab);
        RerollSingle(item);
    }

    public void Reroll() {
        foreach (var item in Items) {
            RerollSingle(item);
        }
    }

    private void RerollSingle(ShopItem item) {
        var sell = _manager.GetShopItem();
        item.gameObject.SetActive(sell != null);
        if (sell == null) {
            return;
        }
        item.Icon.sprite = sell.Icon;
        item.Text.text = sell.Description;
        item.Price = (int)sell.PriceRange.Evaluate(LevelManager.CurrentLevel);
        item.PriceText.text = item.Price.ToString();
        item.Item = sell;
        var weapon = sell.Prefab.GetComponent<Weapon>();
        if (weapon) {
            if (weapon.SecondsPerCycle > 1) {
                item.Text.text += $"\nFire rate: {weapon.SecondsPerCycle:N1}s";
            } else {
                item.Text.text += $"\nFire rate: {1/weapon.SecondsPerCycle:N1}/s";
            }

            item.Text.text += $"\nDamage: {weapon.AttacksPerCycle}x{weapon.DamageText}";
        }
    }

    private void OnDestroy() {
        if (gameObject.activeSelf) {
            Time.timeScale = 1;
        }
    }

    public void Open(LevelManager manager) {
        _manager = manager;
        Time.timeScale = 0;
        Reroll();
        gameObject.SetActive(true);
    }

    public void Close() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}