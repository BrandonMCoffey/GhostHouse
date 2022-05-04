using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerVisualSwapper : MonoBehaviour
{
    [SerializeField] private List<GameObject> _keyboardMouseArt = new List<GameObject>();
    [SerializeField] private List<GameObject> _controllerArt = new List<GameObject>();

    private bool _controller;

    private void Start() {
        Swap(false);
    }

    private void Update() {
        if (_controller != CameraController.UsingController) {
            Swap(CameraController.UsingController);
        }
    }

    private void Swap(bool controller) {
        _controller = controller;
        foreach (var art in _keyboardMouseArt) {
            art.SetActive(!controller);
        }
        foreach (var art in _controllerArt) {
            art.SetActive(controller);
        }
    }
}