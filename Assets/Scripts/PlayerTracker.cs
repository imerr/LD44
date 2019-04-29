using UnityEngine;

public class PlayerTracker : MonoBehaviour {
    [Range(0.1f, 0.75f)]
    public float ViewPortLimit = 0.3f;
    private Camera _camera;

    private void Awake() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {
        if (Player.Instance && _camera) {
            var playerPos = Player.Instance.transform.position;
            var limit = _camera.ViewportToWorldPoint(new Vector2(ViewPortLimit, 0));
            if (playerPos.x - limit.x > 0.05) {
                Vector3 pos;
                Vector3 target = pos = transform.position;
                target.x += playerPos.x - limit.x;
                transform.position = Vector3.Lerp(pos, target, Time.deltaTime * 2);
            }
        }
    }
}