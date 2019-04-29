using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    public static ObjectPool Instance;
    private Dictionary<GameObject, List<GameObject>> _pool = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, GameObject> _inUse = new Dictionary<GameObject, GameObject>();

    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public GameObject Get(GameObject prefab) {
        if (!_pool.ContainsKey(prefab)) {
            _pool.Add(prefab, new List<GameObject>());
        }

        var list = _pool[prefab];
        GameObject o;
        if (list.Count > 0) {
            o = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            o.SetActive(true);
        } else {
            o = Instantiate(prefab);
        }
        _inUse.Add(o, prefab);
        return o;
    }

    public void Release(GameObject o) {
        if (!_inUse.ContainsKey(o)) {
            return;
        }
        var prefab = _inUse[o];
        _inUse.Remove(o);
        _pool[prefab].Add(o);
        o.SetActive(false);
        o.transform.SetParent(null);
    }
}