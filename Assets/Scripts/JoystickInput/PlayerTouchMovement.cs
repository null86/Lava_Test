using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerTouchMovement : MonoBehaviour {

    [SerializeField] private Vector2 joystickSize = new Vector2(300, 300);
    [SerializeField] private FloatingJoystick joystick;
    private Finger movementFinger;
    private Vector2 movementAmount;

    private void OnEnable() {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += onFingerDown;
        Touch.onFingerMove += onFingerMove;
        Touch.onFingerUp += onFingerUp;
    }

    private void OnDisable() {
        Touch.onFingerDown -= onFingerDown;
        Touch.onFingerMove -= onFingerMove;
        Touch.onFingerUp -= onFingerUp;
        EnhancedTouchSupport.Disable();
    }

    private void onFingerUp(Finger finger) {
        if (finger == movementFinger) {
            movementFinger = null;
            joystick.knob.anchoredPosition = Vector2.zero;
            joystick.gameObject.SetActive(false);
            movementAmount = Vector2.zero;
        }
    }

    private void onFingerMove(Finger finger) {
        if (finger == movementFinger) {
            Vector2 knobPosition;
            float maxMovement = joystickSize.x / 2f;
            Touch currentTouch = finger.currentTouch;

            if (Vector2.Distance
                (currentTouch.screenPosition,
                joystick.rectTransform.anchoredPosition) > maxMovement) {
                knobPosition = (currentTouch.screenPosition -
                    joystick.rectTransform.anchoredPosition).normalized * maxMovement;
            } else {
                knobPosition = currentTouch.screenPosition - joystick.rectTransform.anchoredPosition;
            }
            joystick.knob.anchoredPosition = knobPosition;
            movementAmount = knobPosition / maxMovement;
        }
    }

    private void onFingerDown(Finger finger) {
        movementFinger = finger;
        movementAmount = Vector2.zero;
        joystick.gameObject.SetActive(true);
        joystick.rectTransform.sizeDelta = joystickSize;
        joystick.rectTransform.anchoredPosition = ClampStartPosition(finger.screenPosition);

    }

    private Vector2 ClampStartPosition(Vector2 startPosition) {
        if (startPosition.x < joystickSize.x / 2)
            startPosition.x = joystickSize.x / 2;

        if (startPosition.y < joystickSize.y / 2)
            startPosition.y = joystickSize.y / 2;
        else if (startPosition.y > Screen.height - joystickSize.y / 2)
            startPosition.y = Screen.height - joystickSize.y / 2;

        return startPosition;
    }

    [SerializeField] GameObject targetObject;

    private void Update() {
        targetObject.transform.position = new Vector3(transform.position.x + movementAmount.x, transform.position.y, transform.position.z + movementAmount.y);
    }
}



