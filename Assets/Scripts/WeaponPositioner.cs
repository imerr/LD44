using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPositioner : MonoBehaviour {
    [Serializable]
    public struct Container {
        public Transform Transform;
        public Vector2 Offset;
    }

    public Container[] Containers;
    private int _nextContainer;

    public void Add(GameObject prefab) {
        var go = Instantiate(prefab);
        var weapon = go.GetComponent<Weapon>();
        if (!weapon) {
            Debug.LogWarning("Invalid weapon prefab " + prefab);
            Destroy(go);
            return;
        }

        var t = go.transform;
        t.SetParent(Containers[_nextContainer].Transform, false);
        t.localPosition = Containers[_nextContainer].Offset * (Containers[_nextContainer].Transform.childCount - 1);
        t.eulerAngles = Vector3.zero;
        _nextContainer = (_nextContainer + 1) % Containers.Length;
        
        var owner = GetComponent<IWeaponOwner>();
        if (owner is Player player) {
            weapon.Initialize(player);
        } else if (owner is Enemy enemy) {
            weapon.Initialize(enemy);
        }
    }
}