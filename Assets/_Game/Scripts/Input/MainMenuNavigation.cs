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
    [SerializeField] private GameObject _returnFromOptionsButton = null;
    [SerializeField] private GameObject _confirmResetButton = null;
    [SerializeField] private GameObject _returnConfirmResetButton = null;
    [SerializeField] private GameObject _confirmQuitButton = null;
    [SerializeField] private GameObject _returnConfirmQuitButton = null;
    [SerializeField] private GameObject _confirmNewGameButton = null;

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
        SetMenu(_startGameButton);
    }

    public void NavigateOptions1() {
        SetMenu(_options1FirstButton);
    }

    public void NavigateOptions2() {
        SetMenu(_options2FirstButton);
    }

    public void ReturnFromOptions() {
        SetMenu(_returnFromOptionsButton);
    }

    public void NavigateConfirmReset() {
        SetMenu(_confirmResetButton);
    }

    public void ReturnFromReset() {
        SetMenu(_returnConfirmResetButton);
    }

    public void NavigateConfirmQuit() {
        SetMenu(_confirmQuitButton);
    }

    public void ReturnFromConfirmQuit() {
        SetMenu(_returnConfirmQuitButton);
    }

    public void NavigateConfirmNewGame() {
        SetMenu(_confirmNewGameButton);
    }

    private static void SetMenu(GameObject firstSelected) {
        if (firstSelected == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}