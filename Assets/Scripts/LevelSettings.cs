using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Settings", order = 1)]
public class LevelSettings : ScriptableObject {
    [Serializable]
    public class ChanceByLevel {
        public AnimationCurve SpawnChanceByLevel;
    }
    [Serializable]
    public class EnemySetting : ChanceByLevel{
        public GameObject[] Prefabs;
        [Range(1, 10)]
        public int WeaponCountMin = 1;
        [Range(1, 10)]
        public int WeaponCountMax = 1;
        public GameObject[] Weapons;
    }
    [Serializable]
    public class ShopSetting : ChanceByLevel {
        public GameObject Prefab;
        public Sprite Icon;
        [TextArea]
        public string Description;
        public AnimationCurve PriceRange;
    }
    public EnemySetting[] Enemies;
    public ShopSetting[] ShopItems;
    public AnimationCurve RerollCost;
    public EnemySetting[] Bosses;
}