using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuNavigation : MonoBehaviour
{
    [SerializeField] private bool _debug = false;
    [SerializeField] private GameObject _startGameButton = null;
    [SerializeField] private GameObject _options1FirstButton = null;
    [SerializeField] private GameObject _options2FirstButton = null;

    private GameObject _current;

    private void Update() {
        if (_debug) {
            var current = EventSystem.current.currentSelectedGameObject;
            if (_current != current) {
                _current = current;
                Debug.Log("Selected: " + current, current);
            }
        }
    }

    public void NavigateMainMenu() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_startGameButton);
    }
    public void NavigateOptions1()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_options1FirstButton);
    }
    public void NavigateOptions2()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_options2FirstButton);
    }
}
