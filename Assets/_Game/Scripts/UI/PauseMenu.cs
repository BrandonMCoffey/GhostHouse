using UnityEngine;
using Utility.Audio.Managers;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Singleton;
    public static System.Action<bool> PauseUpdated;

    [SerializeField] private JournalController _journal = null;

    public bool IsPaused { get; private set; } = false;
    private bool _canPause = true;

    private void Awake() {
        Singleton = this;
    }

    private void Start() {
        IsPaused = false;
        UpdatePaused();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) {
            if (IsPaused) {
                ResumeGame();
            }
            else {
                PauseGame();
            }
        }
        else if (IsPaused) {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                _journal.PreviousPage();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                _journal.NextPage();
            }
        }
    }

    private void UpdatePaused() {
        ModalWindowController.Singleton.HideHudOnPause(IsPaused);
        SoundManager.MusicManager.SetPaused(IsPaused);
        if (_canPause) {
            //IsometricCameraController.Singleton.gamePaused = IsPaused;
        }
        if (_journal != null)
            if (IsPaused) {
                // TODO: Open to Journal Notification!
                {
                    _journal.gameObject.SetActive(true);
                    _journal.OpenJournal();
                }
            }
        PauseUpdated?.Invoke(IsPaused);
    }

    public void PauseGame() {
        if (_canPause && IsPaused) {
            IsPaused = true;
            UpdatePaused();
        }
    }

    public void ResumeGame() {
        if (IsPaused && _journal.ClosePage()) {
            IsPaused = false;
            UpdatePaused();
        }
    }

    public void PreventPausing(bool updateCanPause) {
        // If no longer able to pause but also currently paused, resume
        if (!updateCanPause && IsPaused) {
            ResumeGame();
        }
        _canPause = updateCanPause;
    }
}