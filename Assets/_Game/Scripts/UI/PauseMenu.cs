using System;
using UnityEngine;
using Utility.Audio.Managers;
using Utility.ReadOnly;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Singleton;
    public static Action<bool> PauseUpdated;

    [SerializeField] private JournalController _journal;
    [SerializeField, ReadOnly] private bool _canPause = true;

    public static event Action<bool> UpdateCanPause = delegate { };

    public bool IsPaused { get; private set; }

    private void Awake() {
        Singleton = this;
        IsPaused = false;
        UpdatePaused();
        PreventPausing(false);
    }

    private void Start() {
        if (_journal == null) _journal = FindObjectOfType<JournalController>();
        if (_journal == null) Debug.LogError("Missing Journal Connection!", gameObject);
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
        if (_journal != null) {
            if (IsPaused) {
                // TODO: Open to Journal Notification!
                {
                    _journal.gameObject.SetActive(true);
                    _journal.OpenJournal();
                }
            }
        }
        PauseUpdated?.Invoke(IsPaused);
    }

    public void PauseGame() {
        if (_canPause && !IsPaused) {
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

    public void PreventPausing(bool prevent) {
        // If no longer able to pause but also currently paused, resume
        if (prevent && IsPaused) {
            ResumeGame();
        }
        _canPause = !prevent;
        UpdateCanPause?.Invoke(_canPause);
    }
}