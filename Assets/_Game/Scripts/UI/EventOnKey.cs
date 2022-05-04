using System;
using UnityEngine;
using UnityEngine.Events;

public class EventOnKey : MonoBehaviour
{
    [SerializeField] private Keys _keyCode = Keys.None;
    [SerializeField] private UnityEvent _event = new UnityEvent();

    private void OnEnable() {
        switch (_keyCode) {
            case Keys.PageLeft:
                UserInput.TurnPageLeft += InvokeEvent;
                break;
            case Keys.PageRight:
                UserInput.TurnPageRight += InvokeEvent;
                break;
            case Keys.ConfirmOrInteract:
                UserInput.ConfirmOrInteract += InvokeEvent;
                break;
            case Keys.AltInteract:
                UserInput.InteractAlt += InvokeEvent;
                break;
            case Keys.OpenJournal:
                UserInput.OpenJournal += InvokeEvent;
                break;
            case Keys.Cancel:
                UserInput.Cancel += InvokeEvent;
                break;
        }
    }

    private void OnDisable() {
        switch (_keyCode) {
            case Keys.PageLeft:
                UserInput.TurnPageLeft -= InvokeEvent;
                break;
            case Keys.PageRight:
                UserInput.TurnPageRight -= InvokeEvent;
                break;
            case Keys.ConfirmOrInteract:
                UserInput.ConfirmOrInteract -= InvokeEvent;
                break;
            case Keys.AltInteract:
                UserInput.InteractAlt -= InvokeEvent;
                break;
            case Keys.OpenJournal:
                UserInput.OpenJournal -= InvokeEvent;
                break;
            case Keys.Cancel:
                UserInput.Cancel -= InvokeEvent;
                break;
        }
    }

    public void InvokeEvent() {
        _event.Invoke();
    }
}

public enum Keys
{
    None,
    PageLeft,
    PageRight,
    ConfirmOrInteract,
    AltInteract,
    OpenJournal,
    Cancel
}