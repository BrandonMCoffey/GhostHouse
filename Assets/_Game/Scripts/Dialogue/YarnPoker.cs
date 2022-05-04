using UnityEngine;

public class YarnPoker : MonoBehaviour
{
    [SerializeField] private Yarn.Unity.DialogueRunner _dialogueRunner = null;

    [SerializeField] private string _nodeName = "";

    [SerializeField] private KeyCode _continueKey = KeyCode.Return;

    private void Update()
    {
        if (Input.GetKeyDown(_continueKey))
        {
            Poke();
        }
    }

    private void Poke()
    {
        if (_dialogueRunner != null && _nodeName != "")
        {
            _dialogueRunner.StartDialogue(_nodeName);
        }
    }
}
