using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CreditsManager : MonoBehaviour
{
    [SerializeField] private float _speed = 2.5f;
    private bool _useSpeed;

    private Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        UserInput.ConfirmOrInteract += ToggleSpeed;
        UserInput.Cancel += EndCredits;
    }

    private void OnDisable() {
        UserInput.ConfirmOrInteract -= ToggleSpeed;
        UserInput.Cancel -= EndCredits;
    }

    private void Update() {
        // BUG: Why is this inverted?
        if (!_animator.GetBool("Done")) {
            EndCredits();
        }
    }

    private void ToggleSpeed() {
        _useSpeed = !_useSpeed;
        _animator.speed = _useSpeed ? _speed : 1;
    }

    private static void EndCredits() {
        DataManager.SceneLoader.LoadScene("MainMenu");
    }
}