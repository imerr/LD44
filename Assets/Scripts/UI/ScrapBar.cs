using TMPro;
using UnityEngine;

public class ScrapBar : MonoBehaviour {
    public RectTransform Bar;

    public TextMeshProUGUI Text;
    void Update() {
        int scrap = Player.Instance ? Player.Instance.Health : 0;
        Text.text = $"{scrap}/{Player.ScrapLimit}";
        Bar.anchorMax = new Vector2(((float)scrap)/Player.ScrapLimit, Bar.anchorMax.y);
    }
}