using UnityEngine;

public class TriggerDamager : Damager {
    private void OnTriggerStay2D(Collider2D other) {
        if (!gameObject) {
            return;
        }

        if (Finder.Is(other)) {
            var d = other.GetComponent<IDamagable>();
            DealDamageTo(d);
        }
    }
}