using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {
    public int Damage = 5;
    public int MaxHits = 1;
    public float RehitInterval = 1000;
    public GameObject HitEffect;
    protected TargetFinder Finder;
    public float DestroyAfter = 10;
    private int _hits;
    private IWeaponOwner _owner;
    private HashSet<IDamagable> _damaged;

    public string DamageText {
        get {
            if (RehitInterval > DestroyAfter) {
                return Damage.ToString();
            } else {
                return (int) (Damage / RehitInterval) + "/s";
            }
        }
    }


    public void Initialize(IWeaponOwner owner, TargetFinder finder) {
        Finder = finder;
        _owner = owner;
        _hits = 0;
        _damaged?.Clear();
        StartCoroutine(nameof(AutoDestroy));
        if (RehitInterval < DestroyAfter) {
            StartCoroutine(nameof(ClearDamaged));
        }
    }

    private IEnumerator AutoDestroy() {
        yield return new WaitForSeconds(DestroyAfter);
        Release();
    }

    public void Release() {
        ObjectPool.Instance.Release(gameObject);
        StopCoroutine(nameof(ClearDamaged));
        StopCoroutine(nameof(AutoDestroy));
    }

    public IEnumerator ClearDamaged() {
        while (gameObject) {
            _damaged?.Clear();
            yield return new WaitForSeconds(RehitInterval);
        }
    }
    
    protected void DealDamageTo(IDamagable target) {
        if (_damaged == null) {
            _damaged = new HashSet<IDamagable>();
        }

        if (!_damaged.Contains(target)) {
            _damaged.Add(target);
            target.ReceiveDamage(Damage, _owner);
            if (HitEffect) {
                var t = transform;
                Instantiate(HitEffect, t.position, Quaternion.FromToRotation(Vector3.right, -t.right));
            }

            _hits++;
            if (_hits >= MaxHits && isActiveAndEnabled) {
                Release();
            }
        }
    }
}