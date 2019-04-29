using UnityEngine;

public class BeamWeapon : Weapon {
    public GameObject Beam;
    public Transform ShootOrigin;
    public override string DamageText => Beam.GetComponent<Damager>()?.DamageText;
    protected override void Attack(float angle) {
        var o = ObjectPool.Instance.Get(Beam);
        o.transform.parent = transform;
        o.transform.position = (ShootOrigin ? ShootOrigin : transform).position;
        o.transform.rotation = Quaternion.Euler(0, 0, angle);
        o.transform.localScale = new Vector3(Range, 1, 1);
        o.GetComponent<Damager>()?.Initialize(Owner, TargetFinder);
    }
}