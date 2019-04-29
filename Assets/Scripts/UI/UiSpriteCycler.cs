using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiSpriteCycler : MonoBehaviour {
    public Sprite[] Sprites;

    public float Speed;
    private Image _image;

    private void Start() {
        _image = GetComponent<Image>();
        StartCoroutine(Cycle());
    }

    IEnumerator Cycle() {
        while (true) {
            foreach (var sprite in Sprites) {
                _image.sprite = sprite;
                yield return new WaitForSecondsRealtime(Speed);
            }
        }
    }
}