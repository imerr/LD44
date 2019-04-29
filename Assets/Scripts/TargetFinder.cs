using System;
using System.Linq;
using UnityEngine;

public struct TargetFinder {
    public int Mask;
    private Type Type;
    private static Collider2D[] _tmpOverlaps = new Collider2D[100];
    private static RaycastHit2D[] _tmpRaycastResults = new RaycastHit2D[100];

    public TargetFinder(int layerMask, Type t) {
        Mask = layerMask;
        Type = t;
    }

    public Component Find(Vector3 myPos, float range) {
        var overlaps = Physics2D.OverlapCircleNonAlloc(myPos, range, _tmpOverlaps, Mask);
        Component closest = null;
        float closestDist = float.PositiveInfinity;
        for (int i = 0; i < overlaps; i++) {
            var c = _tmpOverlaps[i].GetComponent(Type);
            var pos = c.transform.position;
            var dist = (myPos - pos).sqrMagnitude;
            closest = c;
            if (c && dist < closestDist) {
                closestDist = dist;
            }
        }

        return closest;
    }

    public bool Is(Collider2D other) {
        return ((1 << other.gameObject.layer) & Mask) != 0 && other.GetComponent(Type);
    }

    public void Raycast(Vector2 position, Vector3 direction, float length, Action<IDamagable> action) {
        var hits = Physics2D.RaycastNonAlloc(position, direction, _tmpRaycastResults, length, Mask);
        var self = this;
        foreach (var hit in _tmpRaycastResults.Take(hits).Where(d => self.Is(d.collider))
            .OrderBy(d => (position - d.point).magnitude)) {
            action(hit.collider.GetComponent<IDamagable>());
        }
    }
}