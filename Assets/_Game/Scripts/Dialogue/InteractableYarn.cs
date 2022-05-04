using UnityEngine;
using Yarn.Unity;

public class InteractableYarn : InteractableBase
{
    private static DialogueRunner _dialogueRunner;
    private static DialogueRunner DialogueRunner
    {
        get
        {
            if (_dialogueRunner == null)
            {
                _dialogueRunner = FindObjectOfType<DialogueRunner>();
            }
            return _dialogueRunner;
        }
    }

    [SerializeField] private string yarnNode = "";

    public override void OnLeftClick()
    {
        DialogueRunner.StartDialogue(yarnNode);
    }
}