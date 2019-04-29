using UnityEngine;

public class ProjectileWeapon : Weapon {
    public GameObject Projectile;
    public Transform ShootOrigin;

    public override string DamageText => Projectile?.GetComponent<Damager>()?.DamageText;

    protected override void Attack(float angle) {
        var o = ObjectPool.Instance.Get(Projectile);
        o.transform.position = (ShootOrigin ? ShootOrigin : transform).position;
        o.transform.rotation = Quaternion.Euler(0, 0, angle);
        o.GetComponent<Damager>()?.Initialize(Owner, TargetFinder);
    }
}