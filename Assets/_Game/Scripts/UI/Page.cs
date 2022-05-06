using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Page : MonoBehaviour
{
    [SerializeField] private bool _debug = false;
    [SerializeField] private GameObject _firstSelected = null;

    private bool _usingController;

    public void OnEnable() {
        SetFirstSelected();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait() {
        yield return null;
        SetFirstSelected();
    }

    private void Update() {
        if (_usingController == CameraController.UsingController) return;
        _usingController = CameraController.UsingController;
        if (_usingController) {
            if (_debug) Debug.Log("Switched to controller, set first selected");
            SetFirstSelected();
        }
    }

    public void SetFirstSelected() {
        SetMenu(_firstSelected);
    }

    public void SetSelected(GameObject selected) {
        SetMenu(selected);
    }

    private void SetMenu(GameObject firstSelected) {
        if (firstSelected == null) return;
        if (_debug) Debug.Log("Set Selected: " + firstSelected.name, firstSelected);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}