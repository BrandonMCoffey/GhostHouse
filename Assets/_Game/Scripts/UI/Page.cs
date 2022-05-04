using UnityEngine;
using UnityEngine.EventSystems;

public class Page : MonoBehaviour
{
    [SerializeField] private GameObject _firstSelected = null;

    public void OnEnable() {
        SetFirstSelected();
    }

    public void SetFirstSelected() {
        SetMenu(_firstSelected);
    }

    public void SetSelected(GameObject selected) {
        SetMenu(selected);
    }

    private static void SetMenu(GameObject firstSelected)
    {
        if (firstSelected == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
