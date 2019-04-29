using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class Weapon : MonoBehaviour {
    public enum TargetModes {
        FixedAngle,
        Cursor,
        NearestTarget
    }

    [FormerlySerializedAs("CyclesPerSecond")] public float SecondsPerCycle = 0.5f;
    public float AttacksPerCycle = 1;
    [Range(0, 90)]
    public float AttackSpread;
    public TargetModes TargetMode = TargetModes.FixedAngle;
    [Range(-360, 360)] public int MinAttackAngle;
    [Range(-360, 360)] public float AttackAngle;
    [Range(-360, 360)] public int MaxAttackAngle;
    public float FixedAngleVelocity = 20;
    public bool FixedAngleBounce;
    public float Range = 100;
    public bool RotateToTarget = true;
    protected TargetFinder TargetFinder;
    private bool _fixedAngleBouncing;
    private float _attackTimer;
    protected IWeaponOwner Owner;
    private bool _hasTarget;
    public abstract string DamageText { get; }

    public virtual void Initialize(Player player) {
        Owner = player;
        TargetFinder = new TargetFinder(LayerMask.GetMask("Enemy"), typeof(Enemy));
    }

    public virtual void Initialize(Enemy enemy) {
        Owner = enemy;
        TargetFinder = new TargetFinder(LayerMask.GetMask("Player"), typeof(Player));
    }

    protected virtual void Update() {
        UpdateTargetAngle();
        _attackTimer += Time.deltaTime;
        if (_attackTimer > SecondsPerCycle && !CanAttack()) {
            _attackTimer = SecondsPerCycle;
            return;
        }

        while (_attackTimer > SecondsPerCycle) {
            _attackTimer -= SecondsPerCycle;
            for (int i = 0; i < AttacksPerCycle; i++) {
                Attack(AttackAngle + Random.Range(-AttackSpread, AttackSpread) - transform.parent.eulerAngles.z);
            }
        }

        if (RotateToTarget) {
            var t = transform;
            var a = t.eulerAngles;
            a.z = AttackAngle;
            a.z -= transform.parent.eulerAngles.z;
            t.eulerAngles = a;
        }
    }

    protected virtual void UpdateTargetAngle() {
        Vector3 targetPos = transform.position + Vector3.right * 1;
        switch (TargetMode) {
            case TargetModes.FixedAngle:
                if (MinAttackAngle != MaxAttackAngle) {
                    if (_fixedAngleBouncing) {
                        AttackAngle -= FixedAngleVelocity * Time.deltaTime;
                        if (AttackAngle <= MinAttackAngle) {
                            AttackAngle = MinAttackAngle;
                            _fixedAngleBouncing = false;
                        }
                    } else {
                        AttackAngle += FixedAngleVelocity * Time.deltaTime;
                        if (AttackAngle >= MaxAttackAngle) {
                            if (FixedAngleBounce) {
                                AttackAngle = MaxAttackAngle;
                                _fixedAngleBouncing = true;
                            } else {
                                AttackAngle = MinAttackAngle;
                            }
                        }
                    }
                }
                    return;
            case TargetModes.Cursor:
                var c = Camera.main;
                if (c != null) {
                    targetPos = c.ScreenToWorldPoint(Input.mousePosition);
                    targetPos.z = 0;
                }

                break;
            case TargetModes.NearestTarget:
                var found = TargetFinder.Find(transform.position, Range);
                _hasTarget = found;
                if (found) {
                    targetPos = found.transform.position;
                    targetPos.z = 0;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        AttackAngle = Quaternion.FromToRotation(Vector3.right, targetPos - transform.position).eulerAngles.z - transform.parent.eulerAngles.z;
        //AttackAngle = Mathf.Clamp(Utils.NormalizeAngle(AttackAngle), MinAttackAngle, MaxAttackAngle);
    }

    protected virtual bool CanAttack() {
        return Owner.IsAttacking && (TargetMode != TargetModes.NearestTarget || _hasTarget);
    }

    protected abstract void Attack(float angle);
}