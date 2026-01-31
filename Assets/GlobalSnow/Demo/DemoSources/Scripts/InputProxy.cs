using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace GlobalSnowEffect {
    public static class InputProxy {
        public static float GetAxis (string axis) {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null) return 0;
            switch (axis) {
                case "Mouse X":
                    return Mouse.current != null ? Mouse.current.delta.x.ReadValue() * 0.1f : 0;
                case "Mouse Y":
                    return Mouse.current != null ? Mouse.current.delta.y.ReadValue() * 0.1f : 0;
                case "Vertical":
                    if (Keyboard.current.wKey.isPressed) return 1f;
                    if (Keyboard.current.sKey.isPressed) return -1f;
                    return 0;
                case "Horizontal":
                    if (Keyboard.current.dKey.isPressed) return 1f;
                    if (Keyboard.current.aKey.isPressed) return -1f;
                    return 0;
            }
            return 0;
#else
            return Input.GetAxis(axis);
#endif
        }

        public static bool GetKey (KeyCode key) {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null) return false;
            switch (key) {
                case KeyCode.LeftShift:
                case KeyCode.RightShift:
                    return Keyboard.current.shiftKey.isPressed;
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                    return Keyboard.current.ctrlKey.isPressed;
                case KeyCode.Q:
                    return Keyboard.current.qKey.isPressed;
                case KeyCode.E:
                    return Keyboard.current.eKey.isPressed;
                case KeyCode.A:
                    return Keyboard.current.aKey.isPressed;
                case KeyCode.D:
                    return Keyboard.current.dKey.isPressed;
                case KeyCode.W:
                    return Keyboard.current.wKey.isPressed;
                case KeyCode.S:
                    return Keyboard.current.sKey.isPressed;
                case KeyCode.Space:
                    return Keyboard.current.spaceKey.isPressed;
                default:
                    return false;
            }
#else
            return Input.GetKey(key);
#endif
        }

        public static bool GetKeyDown (KeyCode key) {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null) return false;
            switch (key) {
                case KeyCode.T:
                    return Keyboard.current.tKey.wasPressedThisFrame;
                case KeyCode.Space:
                    return Keyboard.current.spaceKey.wasPressedThisFrame;
                case KeyCode.H:
                    return Keyboard.current.hKey.wasPressedThisFrame;
                case KeyCode.Escape:
                    return Keyboard.current.escapeKey.wasPressedThisFrame;
                default:
                    // This is not a comprehensive mapping.
                    // It only covers keys used in DemoWalk.cs.
                    // A more robust solution would involve a full mapping if more keys are needed.
                    return false;
            }
#else
            return Input.GetKeyDown(key);
#endif
        }
    }
}