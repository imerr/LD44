using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IWeaponOwner, IDamagable {
    private TargetFinder _targetFinder;
    public float AggroRange = 50;
    public int Health = 10;
    public int Scrap = 5;
    public GameObject DeathEffect;
    public Vector2 MoveSpeed = new Vector2(5f, 0.5f);
    [EnumFlag]
    public AIFlags Ai;

    public int ContactDamage = 100;

    [Flags]
    public enum AIFlags {
        FlyLeft = 1 << 0,
        FlyUpDown = 1 << 1,
        FlyPlayerHeight = 1 << 2,
    }

    public bool IsAttacking { get; private set; }

    private SpriteRenderer _renderer;
    private Rigidbody2D _body;
    private Vector2 _velocity;
    private bool _flyUp = true;
    private static RaycastHit2D[] _tmpRaycastHits = new RaycastHit2D[100];

    private void Awake() {
        _targetFinder = new TargetFinder(LayerMask.GetMask("Player"), typeof(Player));
        foreach (var weapon in GetComponentsInChildren<Weapon>()) {
            weapon.Initialize(this);
        }

        _renderer = GetComponent<SpriteRenderer>();
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        IsAttacking = _targetFinder.Find(transform.position, AggroRange);
        if (Utils.Camera) {
            var left = Utils.Camera.ViewportToWorldPoint(new Vector2(0, 0));
            if (left.x > transform.position.x + _renderer.sprite.bounds.size.x) {
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate() {
        var pos = transform.position;
        if (Ai.HasFlag(AIFlags.FlyLeft)) {
            _velocity.x = Mathf.Lerp(_velocity.x, -Random.Range(MoveSpeed.x / 2, MoveSpeed.x), Time.deltaTime * 2);
        } else {
            _velocity.x = Mathf.Lerp(_velocity.x, 0, Time.deltaTime * 2);
        }

        if (Ai.HasFlag(AIFlags.FlyUpDown)) {
            if (Player.Instance && Ai.HasFlag(AIFlags.FlyPlayerHeight)) {
                var playerPos = Player.Instance.transform.position;
                float difference = playerPos.y - pos.y;
                if (Mathf.Abs(difference) > 0.1) {
                    _velocity.y = Mathf.Lerp(_velocity.y, (difference > 0 ? 1 : -1) * Random.Range(MoveSpeed.y / 2, MoveSpeed.y), Time.deltaTime * 2);
                } else {
                    _velocity.y = Mathf.Lerp(_velocity.y, 0, Time.deltaTime * 2);
                }
            } else {
                if (Random.Range(0.0f, 1.0f) < 0.01f) {
                    _flyUp = !_flyUp;
                }

                _velocity.y = Mathf.Lerp(_velocity.y, (_flyUp ? 1 : -1) * Random.Range(MoveSpeed.y / 2, MoveSpeed.y),
                    Time.deltaTime * 2);
            }
        }

        // check if we would hit anything
        int hits = Physics2D.RaycastNonAlloc(pos, _velocity.normalized, _tmpRaycastHits, 0.5f,
            LayerMask.GetMask("Player", "Enemy", "Wall"));
        bool collided = false;
        for (int i = 0; i < hits; i++) {
            var hit = _tmpRaycastHits[i];
            if (hit.transform == transform) {
                continue;
            }

            collided = true;
            break;
        }

        if (collided) {
            _velocity = Vector2.Lerp(_velocity, Vector2.zero, Time.deltaTime * 3);
        }

        _body.velocity = _velocity;
    }

    public void ReceiveDamage(int damage, IWeaponOwner owner) {
        if (Health <= 0) {
            // was already dead
            return;
        }

        if (_renderer.bounds.min.x > Utils.Camera.ViewportToWorldPoint(new Vector2(1, 0)).x) {
            // Cant kill things off screen
            return;
        }

        Health -= damage;
        if (Health <= 0) {
            owner?.ReceiveScrap(Scrap);
            Destroy(gameObject);
            Instantiate(DeathEffect, transform.position, Quaternion.identity);
        }
    }

    public void ReceiveScrap(int scrap) {
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (Player.Instance && other.gameObject == Player.Instance.gameObject) {
            Player.Instance.ReceiveDamage(ContactDamage, this);
            ReceiveDamage(Health, null);
        }
    }
}