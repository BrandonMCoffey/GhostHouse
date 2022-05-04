using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Utility.Buttons;
using Utility.ReadOnly;

public class FontManager : MonoBehaviour
{
    public static FontManager Instance;

    [Header("Font Assets")]
    [SerializeField] private TMP_FontAsset _fancyFont = null;
    [SerializeField] private TMP_FontAsset _normalFont = null;
    [SerializeField] private TMP_FontAsset _dyslexiaFont = null;

    [Header("Current")]
    [SerializeField, ReadOnly] private FontMode _curFont = FontMode.Dyslexia;

    private void Awake() {
        // enforce singleton pattern
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this);
            return;
        }
    }

    private void Start() {
        UpdateAllText(_curFont);
        SceneManager.sceneLoaded += (s, m) => UpdateAllText(_curFont);
    }

    /// <summary>
    /// Gets the font asset corresponding to the player's choice in <see cref="DataManager"/>
    /// </summary>
    /// <returns></returns>
    public TMP_FontAsset GetFont(FontMode mode) {
        switch (mode) {
            default:
            case FontMode.Dyslexia:
                return _dyslexiaFont;
            case FontMode.Fancy:
                return _fancyFont;

            case FontMode.Normal:
                return _normalFont;
        }
    }

    /// <summary>
    /// Updates the fonts of text in scene to user preferences.
    /// </summary>
    /// <param name="mode"></param>
    /// <exception cref="System.ArgumentNullException"> Unable to find a font asset corresponding to <paramref name="mode"/> </exception>
    [Button(Mode = ButtonMode.OnlyWhileGameRunning)]
    public void UpdateAllText(FontMode mode) {
        // verify we have a font
        TMP_FontAsset fontAsset = GetFont(mode);
        if (fontAsset == null) throw new System.ArgumentNullException("font is null.");

        // cache mode to allow scene-changes to follow suit
        _curFont = mode;

        // get all text mesh pro components in the scene
        var textInCurScene = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        for (int i = 0; i < textInCurScene.Length; i++) {
            if (string.IsNullOrEmpty(textInCurScene[i].gameObject.scene.name))
                textInCurScene[i] = null;
        }

        foreach (var text in textInCurScene) {
            if (text == null) continue;

            try {
                text.font = fontAsset;
            }
            catch (System.ArgumentException) {
                //Debug.Log("Here is mystery exception! OnlyWhileGameRunning sure why it bug, but it do and it don't matter :).");
            }
        }
    }
}

public enum FontMode
{
    Fancy,
    Normal,
    Dyslexia
}