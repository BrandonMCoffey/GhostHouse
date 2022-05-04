using UnityEngine;

public class LoadOpeningLevel : MonoBehaviour
{
    [SerializeField] private MainMenuNavigation _navigation = null;
    [SerializeField] private GameObject _newGameConfirmation = null;
    [SerializeField] private SimpleFadeToBlack _fade = null;

    public void LoadSandbox() {
        DataManager.Instance.OnNewGame();
        DataManager.SceneLoader.LoadScene("Sandbox");
    }

    public void CheckSave() {
        var savedLevel = DataManager.Instance.level;
        bool continueAvailable = savedLevel == "Summer" || savedLevel == "Fall" || savedLevel == "Winter";

        if (continueAvailable) {
            _newGameConfirmation.SetActive(true);
            if (_navigation != null) _navigation.NavigateConfirmNewGame();
        }
        else {
            LoadSpring();
        }
    }

    public void LoadSpring() {
        _fade.FadeOut();
        DataManager.Instance.OnNewGame();
        DataManager.SceneLoader.LoadScene("Spring");
    }
}