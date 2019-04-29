using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour {
    public LevelSettings Settings;
    public static int CurrentLevel;
    public Shop Shop;
    public float LevelLenght = 50;
    public float SpawnEvery = 1;

    public enum Stages {
        Shop,
        Combat,
        PreBoss,
        Boss,
    }

    public Stages Stage;

    internal class Range<T> where T : LevelSettings.ChanceByLevel {
        internal class Element<T2> where T2 : LevelSettings.ChanceByLevel {
            public float Chance;
            public T2 Value;
        }

        public List<Element<T>> Elements = new List<Element<T>>();
        public float Sum;

        /// <summary>
        /// Builds the chance cache for faster lookups
        /// </summary>
        /// <param name="level"></param>
        /// <param name="settings"></param>
        public void BuildCache(int level, T[] settings) {
            Elements.Clear();
            Sum = 0;
            foreach (var setting in settings) {
                var chance = setting.SpawnChanceByLevel.Evaluate(level);
                if (Mathf.Approximately(chance, 0) || chance < 0) {
                    continue;
                }

                Sum += chance;
                Elements.Add(new Element<T> {
                    Value = setting,
                    Chance = chance
                });
            }
        }

        /// <summary>
        /// returns a random element
        /// </summary>
        /// <returns></returns>
        public T Get() {
            float r = Random.Range(0.0f, Sum);
            float sum = 0;
            foreach (var e in Elements) {
                sum += e.Chance;
                if (sum >= r) {
                    return e.Value;
                }
            }

            return null;
        }
    }

    private Range<LevelSettings.ShopSetting> _shopRange = new Range<LevelSettings.ShopSetting>();
    private Range<LevelSettings.EnemySetting> _enemyRange = new Range<LevelSettings.EnemySetting>();
    private Range<LevelSettings.EnemySetting> _bossRange = new Range<LevelSettings.EnemySetting>();
    private float _lastPos;
    private GameObject _boss;

    // Start is called before the first frame update
    void Start() {
        _shopRange.BuildCache(CurrentLevel, Settings.ShopItems);
        _enemyRange.BuildCache(CurrentLevel, Settings.Enemies);
        _bossRange.BuildCache(CurrentLevel, Settings.Bosses);
        ShowShop();
    }

    private void ShowShop() {
        Shop.Open(this);
    }

    void Update() {
        switch (Stage) {
            case Stages.Shop: {
                if (!Shop.gameObject.activeSelf) {
                    Stage = Stages.Combat;
                }

                break;
            }
            case Stages.Combat: {
                var pos = Utils.Camera.transform.position.x;
                var leftBottom = Utils.Camera.ViewportToWorldPoint(new Vector2(0, 0));
                var rightTop = Utils.Camera.ViewportToWorldPoint(new Vector2(1, 1));

                if (rightTop.x >= LevelLenght - (rightTop.x - leftBottom.x)) {
                    var spawn = _bossRange.Get();
                    _boss = Spawn(spawn, new Vector2(LevelLenght + 1, 0));
                    Stage = Stages.PreBoss;
                    break;
                }

                if (pos - _lastPos >= SpawnEvery) {
                    _lastPos += SpawnEvery;
                    var spawn = _enemyRange.Get();
                    if (Random.Range(0.0f, 1.0f) < spawn.SpawnChanceByLevel.Evaluate(CurrentLevel)) {
                        Spawn(spawn, new Vector2(rightTop.x + 5, Random.Range(leftBottom.y + 1, rightTop.y - 1)));
                    }
                }

                break;
            }
            case Stages.PreBoss: {
                var pos = Utils.Camera.transform.position.x;
                var leftBottom = Utils.Camera.ViewportToWorldPoint(new Vector2(0, 0));
                var rightTop = Utils.Camera.ViewportToWorldPoint(new Vector2(1, 1));
                if (pos >= LevelLenght) {
                    Utils.Camera.GetComponent<PlayerTracker>().enabled = false;
                    Stage = Stages.Boss;
                    var go = new GameObject("Wall");
                    go.layer = LayerMask.NameToLayer("PlayerWall");
                    go.transform.position = rightTop;
                    var collider = go.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(0.05f, rightTop.y - leftBottom.y);
                    collider.offset = new Vector2(0, -collider.size.y / 2);
                }

                break;
            }
            case Stages.Boss: {
                if (!_boss && Player.Instance) {
                    StartCoroutine(NextLevel());
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator NextLevel() {
        yield return new WaitForSeconds(5);
        DontDestroyOnLoad(Player.Instance.gameObject);
        Player.Instance.transform.position = new Vector2(0,0);
        CurrentLevel++;
        SceneManager.LoadScene("Game");
    }

    private GameObject Spawn(LevelSettings.EnemySetting spawn, Vector2 position) {
        var go = Instantiate(spawn.Prefabs[Random.Range(0, spawn.Prefabs.Length)]);
        var w = go.GetComponent<WeaponPositioner>();
        if (w && spawn.Weapons != null) {
            int weapons = Random.Range(spawn.WeaponCountMin, spawn.WeaponCountMax);
            for (int i = 0; i < weapons; i++) {
                w.Add(spawn.Weapons[Random.Range(0, spawn.Weapons.Length)]);
            }
        }

        go.transform.position = position;
        return go;
    }

    public LevelSettings.ShopSetting GetShopItem() {
        return _shopRange.Get();
    }
}