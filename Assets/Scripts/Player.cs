using UnityEngine;

public class Player : MonoBehaviour, IWeaponOwner, IDamagable {
    [Header("Base settings")]
    public Vector2 MovementSpeed = new Vector2(5,5);
    /// <summary>
    /// The current health
    /// 0 = dead
    /// </summary>
    public int Health = 5;
    /// <summary>
    /// Is the player firing its weapons
    /// </summary>
    public bool IsAttacking { get; private set; }

    public static Player Instance;
    public const int ScrapLimit = 1000;

    private Rigidbody2D _body;

    [Header("VFX")]
    public ParticleSystem LeftThruster;
    public ParticleSystem RightThruster;
    public ParticleSystem UpThruster;
    public ParticleSystem DownThruster;
    public GameObject DeathEffect;
    public Transform WeaponContainer;

    private void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }
        _body = GetComponent<Rigidbody2D>();
        IsAttacking = true;
        foreach (var weapon in WeaponContainer.GetComponentsInChildren<Weapon>()) {
            weapon.Initialize(this);
        }

        Instance = this;
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    void FixedUpdate() {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _body.velocity = Vector2.Scale(move, MovementSpeed);
        
        SetThrusterState(move);
    }

    private void SetThrusterState(Vector2 move) {
        if (move.x > 0.01) {
            if (!RightThruster.isPlaying) {
                RightThruster.Play(true);
            }

            LeftThruster.Stop(true);
        } else if (move.x < -0.01) {
            RightThruster.Stop(true);
            if (!LeftThruster.isPlaying) {
                LeftThruster.Play(true);
            }
        } else {
            RightThruster.Stop(true);
            LeftThruster.Stop(true);
        }

        if (move.y > 0.01) {
            if (!UpThruster.isPlaying) {
                UpThruster.Play(true);
            }

            DownThruster.Stop(true);
        } else if (move.y < -0.01) {
            UpThruster.Stop(true);
            if (!DownThruster.isPlaying) {
                DownThruster.Play(true);
            }
        } else {
            UpThruster.Stop(true);
            DownThruster.Stop(true);
        }
    }

    public void ReceiveDamage(int damage, IWeaponOwner owner) {
        Health -= damage;
        if (Health <= 0) {
            LevelManager.CurrentLevel = 0;
            Instantiate(DeathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    
    public void ReceiveScrap(int scrap) {
        Health = Mathf.Clamp(Health + scrap, 0, ScrapLimit);
    }
}