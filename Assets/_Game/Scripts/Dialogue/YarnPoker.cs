using UnityEngine;

public class YarnPoker : MonoBehaviour
{
    [SerializeField] private Yarn.Unity.DialogueRunner _dialogueRunner = null;

    [SerializeField] private string _nodeName = "";

    private void OnEnable() {
        UserInput.ConfirmOrInteract += Poke;
    }

    private void OnDisable() {
        UserInput.ConfirmOrInteract -= Poke;
    }

    private void Poke() {
        if (_dialogueRunner != null && _nodeName != "") {
            _dialogueRunner.StartDialogue(_nodeName);
        }
    }
}