using UnityEngine;

public static class Utils {
    public static Camera Camera {
        get {
            if (_internalCamera && _internalCamera.enabled) {
                return _internalCamera;
            }

            return _internalCamera = Camera.main;
        }
    }

    private static Camera _internalCamera;
    public static float NormalizeAngle(float a) {
        return a % 360;
    }
}