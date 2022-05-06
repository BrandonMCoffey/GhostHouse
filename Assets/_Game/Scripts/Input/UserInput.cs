using System;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [SerializeField] private bool _debug = false;

    // Left Arrow Key or Left Bumper
    public static event Action TurnPageLeft = delegate { };

    // Right Arrow Key or Right Bumper
    public static event Action TurnPageRight = delegate { };

    // Enter, Space, or A
    public static event Action ConfirmOrInteract = delegate { };

    // Left Click or A
    public static event Action Interact = delegate { };

    // Backspace or X
    public static event Action InteractAlt = delegate { };

    // Escape or Y
    public static event Action OpenJournal = delegate { };

    // Escape or B
    public static event Action Cancel = delegate { };

    // B
    public static event Action CancelController = delegate { };

    // End or Back
    public static event Action CheatMenu = delegate { };

    // Start
    public static event Action CheatMenuActivate = delegate { };

    public static bool Sprinting;

    public static float Horizontal => Input.GetAxisRaw("HorizontalKeyboard");
    public static float Vertical => Input.GetAxisRaw("VerticalKeyboard");
    public static float HorizontalController => Input.GetAxis("HorizontalController");
    public static float VerticalController => Input.GetAxis("VerticalController");
    public static float HorizontalController2 => Input.GetAxis("HorizontalController2");
    public static float VerticalController2 => Input.GetAxis("VerticalController2");
    public static float MouseScrollWheel => Input.GetAxis("Mouse ScrollWheel");
    public static Vector3 MousePosition => Input.mousePosition;

    private void Update() {
        if (Input.GetButtonDown("Submit")) {
            ConfirmOrInteract?.Invoke();
            Log("ConfirmOrInteract");
        }
        if (Input.GetButtonDown("Interact")) {
            Interact?.Invoke();
            Log("Interact");
        }
        if (Input.GetButtonDown("InteractAlt")) {
            InteractAlt?.Invoke();
            Log("InteractAlt");
        }
        if (Input.GetButtonDown("Cancel")) {
            Cancel?.Invoke();
            Log("Cancel");
        }
        if (Input.GetButtonDown("CancelController")) {
            CancelController?.Invoke();
            Log("CancelController");
        }
        if (Input.GetButtonDown("OpenJournal")) {
            OpenJournal?.Invoke();
            Log("OpenJournal");
        }
        if (Input.GetButtonDown("PageLeft")) {
            TurnPageLeft?.Invoke();
            Log("PageLeft");
        }
        if (Input.GetButtonDown("PageRight")) {
            TurnPageRight?.Invoke();
            Log("PageRight");
        }
        if (Input.GetButtonDown("CheatMenu")) {
            CheatMenu?.Invoke();
            Log("CheatMenu");
        }
        if (Input.GetButtonDown("Sprint")) {
            Sprinting = true;
        }
        if (Input.GetButtonUp("Sprint")) {
            Sprinting = false;
        }
        if (Input.GetButtonDown("CheatMenuActivate")) {
            CheatMenuActivate?.Invoke();
            Log("CheatMenuActivate");
        }
    }

    private void Log(string input) {
        if (_debug) Debug.Log("Pressed: " + input, gameObject);
    }
}