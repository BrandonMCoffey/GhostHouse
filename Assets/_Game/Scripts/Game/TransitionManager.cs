﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Mechanics.Level_Mechanics;

namespace Game
{
    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] private Image _fadeToBlack = null;
        [SerializeField] private CanvasGroup _titleBanner = null;
        [SerializeField] private GameObject _raycastBlock = null;

        [Header("Spirit Points")]
        [SerializeField] private int _spiritPointsForLevel = 3;
        //[SerializeField] private Integer _spiritPoints = null;

        [Header("On Scene Load")]
        [SerializeField] private string _currentScene = "Spring";
        [SerializeField] private bool _fadeIn = true;
        [SerializeField] private float _fadeInTime = 1;
        [SerializeField] private bool _showTitleText = true;
        [SerializeField] private float _titleTextTime = 1;
        [SerializeField] private AnimationCurve _titleTextVisibility = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 1), new Keyframe(0.75f, 1), new Keyframe(1, 0));
        public Interactable _interactionOnStart = null;

        [Header("On Scene End")]
        [SerializeField] private string _nextScene = "MainMenu";
        [SerializeField] private bool _fadeOut = true;
        [SerializeField] private float _fadeOutTime = 1;

        public static Action OnLevelComplete = delegate { };

        private static DialogueRunner _dialogueRunner;
        public static DialogueRunner DialogueRunner {
            get {
                if (_dialogueRunner == null) {
                    _dialogueRunner = FindObjectOfType<DialogueRunner>();
                }
                return _dialogueRunner;
            }
        }

        private void Start() {
            // Set Spirit Points
            DataManager.Instance.remainingSpiritPoints = _spiritPointsForLevel;
            DataManager.Instance.level = _currentScene;
            //if (_spiritPoints != null) _spiritPoints.value = _spiritPointsForLevel;

            // Intro Sequence
            if (_raycastBlock != null) _raycastBlock.gameObject.SetActive(true);
            CameraController.IsTransitioning = true;
            if (_fadeIn) {
                FadeFromBlack();
                PauseGame(true);
            }
            else if (_showTitleText) {
                TitleText();
                PauseGame(true);
            }
        }

        public void Transition() {
            if (_fadeOut && _fadeToBlack != null) {
                StartCoroutine(FadeToBlack(_fadeOutTime));
            }
            else {
                NextScene();
            }
        }

        private void FadeFromBlack() {
            if (_fadeToBlack == null) {
                if (_showTitleText) TitleText();
                else StartDialogue();
                return;
            }
            StartCoroutine(FadeFromBlack(_fadeInTime));
        }

        private IEnumerator FadeFromBlack(float time) {
            _fadeToBlack.gameObject.SetActive(true);
            for (float t = 0; t < time; t += Time.deltaTime) {
                float delta = 1 - t / time;
                _fadeToBlack.color = new Color(0, 0, 0, delta);
                yield return null;
            }
            _fadeToBlack.gameObject.SetActive(false);
            if (_showTitleText) TitleText();
            else StartDialogue();
        }

        private void TitleText() {
            if (_titleBanner == null) {
                StartDialogue();
                return;
            }
            StartCoroutine(FadeTitleText(_titleTextTime));
        }

        private IEnumerator FadeTitleText(float time) {
            _titleBanner.gameObject.SetActive(true);
            for (float t = 0; t < time; t += Time.deltaTime) {
                float delta = t / time;
                _titleBanner.alpha = _titleTextVisibility.Evaluate(delta);
                yield return null;
            }
            _titleBanner.gameObject.SetActive(false);
            StartDialogue();
        }

        private void StartDialogue() {
            CameraController.IsTransitioning = false;
            if (_raycastBlock != null) _raycastBlock.gameObject.SetActive(false);
            if (_interactionOnStart != null) {
                _interactionOnStart.Interact();
            }
            PauseGame(false);
        }

        private static void PauseGame(bool paused) {
            if (PauseMenu.Singleton != null) {
                PauseMenu.Singleton.PreventPausing(paused);
            }
        }

        private IEnumerator FadeToBlack(float time) {
            if (_raycastBlock != null) _fadeToBlack.gameObject.SetActive(true);
            CameraController.IsTransitioning = true;
            for (float t = 0; t < time; t += Time.deltaTime) {
                float delta = t / time;
                _fadeToBlack.color = new Color(0, 0, 0, delta);
                yield return null;
            }
            CameraController.IsTransitioning = false;
            OnLevelComplete?.Invoke();
            NextScene();
        }

        private void NextScene() {
            DataManager.Instance.level = _nextScene;
            DataManager.Instance.WriteFile();
            DataManager.SceneLoader.LoadScene(_nextScene);
        }
    }
}