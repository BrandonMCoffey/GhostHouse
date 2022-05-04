using UnityEngine;
using UnityEngine.EventSystems;

public class Page : MonoBehaviour
{
    [SerializeField] private bool _debug = false;
    [SerializeField] private GameObject _firstSelected = null;

    private GameObject _current;

    public void OnEnable() {
        SetFirstSelected();
    }

    private void Update() {
        var current = EventSystem.current.currentSelectedGameObject;
        if (_current != current) {
            if (current == null) {
                SetMenu(_current);
                return;
            }
            _current = current;
            if (_debug) Debug.Log("Selected: " + current, current);
        }
    }

    public void SetFirstSelected() {
        SetMenu(_firstSelected);
    }

    public void SetSelected(GameObject selected) {
        SetMenu(selected);
    }

    private static void SetMenu(GameObject firstSelected) {
        if (firstSelected == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}