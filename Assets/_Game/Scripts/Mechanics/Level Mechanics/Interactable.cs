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
        [SerializeField, TextArea] private string _interactableDescription = "Default Description";

        [Header("Interactable Settings")]
        [SerializeField] private string _dialogeYarnNode = "";
        [SerializeField] private int _cost = 0;
        [SerializeField] private bool _canInteractMultipleTimes = false;

        [Header("Other Settings")]
        [SerializeField] private SfxReference _sfxOnInteract = new SfxReference();

        public int Cost => _cost;

        static DialogueRunner _dialogueRunner;
        static DialogueRunner DialogueRunner {
            get {
                if (_dialogueRunner == null) {
                    _dialogueRunner = FindObjectOfType<DialogueRunner>();
                }
                return _dialogueRunner;
            }
        }


        public bool Interacted => DataManager.Instance.GetInteraction(name);
        public bool CanInteract => !Interacted || _canInteractMultipleTimes;

        private List<InteractableResponseBase> _interactableResponses = new List<InteractableResponseBase>();

        public void Raise(InteractableResponseBase response) {
            _interactableResponses.Add(response);
        }

        public void Unraise(InteractableResponseBase response) {
            _interactableResponses.Remove(response);
        }

        [Button(Spacing = 10)]
        public void Interact() {
            //foreach (var response in _interactableResponses) {
            //   response.Invoke();
            //}

            //The same for loop as before, but this one goes backwards to make sure that deleting/removing a interactableResponse doesn't
            //cause any errors.


            for (int i = _interactableResponses.Count - 1; i >= 0; i--) {
                _interactableResponses[i].Invoke();
            }

            if (_cost > 0) {
                // TODO: Apply Spirit Point Cost
                DataManager.Instance.remainingSpiritPoints -= _cost;
            }

            _sfxOnInteract.Play();
            DataManager.Instance.SetInteraction(name, true);

            if (!string.IsNullOrEmpty(_dialogeYarnNode)) {
                DialogueRunner.StartDialogue(_dialogeYarnNode);
            }
        }
    }
}