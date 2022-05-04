using UnityEngine;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    [SerializeField] private Button _button = null;

    private void Start() {
        var season = DataManager.Instance.GetSeason();
        bool validSave = !(season == Season.Spring || season == Season.None);
        if (_button != null)  _button.interactable = validSave;
    }

    public void LoadLevel() {
        var scene = DataManager.Instance.level;
        DataManager.Instance.OnContinueGame();
        DataManager.SceneLoader.LoadScene(scene);
    }
}