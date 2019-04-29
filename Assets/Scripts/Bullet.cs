using UnityEngine;

public class Bullet : Damager {
    public float Speed = 20;
    private void FixedUpdate() {
        var t = transform;
        var pos = t.position;
        var leftBottom = Utils.Camera.ViewportToWorldPoint(new Vector2(0, 0));
        var rightTop = Utils.Camera.ViewportToWorldPoint(new Vector2(1, 1));
        if (pos.x < leftBottom.x - 1 || pos.x > rightTop.x + 1 || 
            pos.y < leftBottom.y - 1 || pos.y > rightTop.y + 1) {
            Release();
            return;
        }
        var right = t.right;
        Finder.Raycast(t.position - right * Speed * Time.deltaTime, right, Speed * Time.deltaTime * 3, DealDamageTo);
        transform.position += right * Speed * Time.deltaTime;
    }
}