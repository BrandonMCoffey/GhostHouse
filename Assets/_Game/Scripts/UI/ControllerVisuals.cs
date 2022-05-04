using UnityEngine;
using Utility.ReadOnly;

public class ControllerVisuals : MonoBehaviour
{
    [SerializeField] private Transform _cursorParent = null;
    [SerializeField, ReadOnly] private bool _isCursorVisible;
    [SerializeField, ReadOnly] private bool _isJournalOpen;

    [SerializeField] private bool _independent;
    [SerializeField, ReadOnly] private bool _usingController;

    private Vector3 _mousePos;

    private void Update() {
        if (_independent) {
            CheckInputType();
        }
        if (_isJournalOpen != PauseMenu.IsPaused) {
            _isJournalOpen = PauseMenu.IsPaused;
            if (_isCursorVisible) {
                ShowCursor(!_isJournalOpen);
            }
        }
    }

    public void ToggleCustomCursor(bool show) {
        Cursor.visible = !show;
        ShowCursor(show && !_isJournalOpen);
        _isCursorVisible = show;
    }

    private void ShowCursor(bool show) {
        if (_cursorParent != null) _cursorParent.gameObject.SetActive(show);
    }

    public void SetCursorPosition(Vector3 pos) {
        if (_cursorParent != null) _cursorParent.position = pos;
    }

    public void SetIndependent(bool independent) {
        _independent = independent;
    }

    private void CheckInputType() {
        var mousePos = UserInput.MousePosition;
        bool mouseMoved = mousePos != _mousePos;
        _mousePos = mousePos;

        if (mouseMoved) {
            ToggleController(false);
        }
        else if (_usingController) {
            if (UserInput.Horizontal + UserInput.Vertical != 0) {
                ToggleController(false);
            }
        }
        else if (UserInput.HorizontalController + UserInput.VerticalController != 0) {
            ToggleController(true);
        }
    }

    private void ToggleController(bool controller) {
        ToggleCustomCursor(controller);
        SetCursorPosition(_mousePos);
        _usingController = controller;
    }
}