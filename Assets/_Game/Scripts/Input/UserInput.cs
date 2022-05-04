using System;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [SerializeField] private bool _debug = false;

    // Left Arrow Key or Left Bumper
    public static event Action TurnPageLeft = delegate { };

    // Right Arrow Key or Right Bumper
    public static event Action TurnPageRight = delegate { };

    // Enter or A
    public static event Action ConfirmOrInteract = delegate { };

    // Backspace or X
    public static event Action InteractAlt = delegate { };

    // Escape or Y
    public static event Action OpenJournal = delegate { };

    // Escape or B
    public static event Action Cancel = delegate { };

    // End or Back
    public static event Action CheatMenu = delegate { };

    public static float Horizontal => Input.GetAxis("Horizontal");
    public static float Vertical => Input.GetAxis("Vertical");

    private void Update() {
        if (Input.GetButtonDown("Submit")) {
            ConfirmOrInteract?.Invoke();
            Log("ConfirmOrInteract");
        }
        if (Input.GetButtonDown("InteractAlt")) {
            InteractAlt?.Invoke();
            Log("InteractAlt");
        }
        if (Input.GetButtonDown("Cancel")) {
            Cancel?.Invoke();
            Log("Cancel");
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
    }

    private void Log(string input) {
        if (_debug) Debug.Log("Pressed: " + input, gameObject);
    }
}