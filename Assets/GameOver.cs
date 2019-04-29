using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    public Button Restart;
    public GameObject Content;

    private void Start() {
        Restart.onClick.AddListener(() => {
            LevelManager.CurrentLevel = 0;
            SceneManager.LoadScene("Game");
        });
    }

    void Update() {
        Content.SetActive(!Player.Instance);
    }
}