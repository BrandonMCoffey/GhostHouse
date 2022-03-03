﻿using System.Collections.Generic;
using UnityEngine;
using Utility.Audio.Helper;
using Utility.Buttons;
using Utility.ReadOnly;
using Yarn.Unity;

namespace Mechanics.Level_Mechanics
{
    [CreateAssetMenu(fileName = "NewInteractable", menuName = "Interactions/Interactable")]
    public class Interactable : ScriptableObject
    {
        //public variables that designers can edit
        [Header("Interactable Name")]
        [SerializeField] private string _interactableName = "Default Name";
        [SerializeField, TextArea] private string _interactableDescription = "Default Description";

        [Header("Interactable Settings")]
        [SerializeField] private string _dialogeYarnNode = "";
        [SerializeField] private int _cost;
        [SerializeField] private bool _canInteractMultipleTimes = false;

        [Header("Other Settings")]
        [SerializeField] private SfxReference _sfxOnInteract = new SfxReference();
        [SerializeField, ReadOnly] public bool _interacted = false;

        public string InteractableInfo => _interactableName + ": " + _interactableDescription;

        static DialogueRunner _dialogueRunner;
        static DialogueRunner DialogueRunner {
            get {
                if (_dialogueRunner == null) {
                    _dialogueRunner = FindObjectOfType<DialogueRunner>();
                }
                return _dialogueRunner;
            }
        }


        public bool CanInteract => !_interacted || _canInteractMultipleTimes;

        private List<InteractableResponse> _interactableResponses = new List<InteractableResponse>();

        public void Raise(InteractableResponse response) {
            _interactableResponses.Add(response);
        }

        public void Unraise(InteractableResponse response) {
            _interactableResponses.Remove(response);
        }

        [Button(Spacing = 10)]
        public void Interact() {
            //foreach (var response in _interactableResponses) {
            //   response.Invoke();
            //}

            //The same for loop as before, but this one goes backwards to make sure that deleting/removing a interactableResponse doesn't
            //cause any errors.


            if (!string.IsNullOrEmpty(_dialogeYarnNode)) {
                DialogueRunner.StartDialogue(_dialogeYarnNode);
            }
            for (int i = _interactableResponses.Count - 1; i >= 0; i--) {
                _interactableResponses[i].Invoke();
            }
            _sfxOnInteract.Play();
            _interacted = true;
            SaveInteraction();
        }

        public void SaveInteraction() {
            DataManager.Instance.SetInteraction(_interactableName, _interacted);
        }

        public void LoadInteraction() {
            _interacted = DataManager.Instance.GetInteraction(_interactableName);
        }
    }
}