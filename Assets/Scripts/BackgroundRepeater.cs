using UnityEngine;

public class BackgroundRepeater : MonoBehaviour {


    public SpriteRenderer BackgroundA;
    public SpriteRenderer BackgroundB;
    private bool _activeA = true;

    // Update is called once per frame
    void Update() {
        if (!Utils.Camera) {
        }

        var leftBottom = Utils.Camera.ViewportToWorldPoint(new Vector3(0, 0));
        var active = _activeA ? BackgroundA : BackgroundB;
        var toMove = _activeA ? BackgroundB : BackgroundA;
        var toMoveTransform = toMove.transform;
        if (leftBottom.x > toMoveTransform.position.x + toMove.sprite.bounds.size.x) {
            var pos = toMoveTransform.position;
            pos.x = active.transform.position.x + active.sprite.bounds.size.x;
            toMoveTransform.position = pos;
            _activeA = !_activeA;
        }
    }
}