using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Communication between the Settings UI and Settings singleton
public class PanelMediator : MonoBehaviour
{
    [SerializeField] private bool SaveOnChange = true;

    // References to UI values
    // Automatically hooks up connections

    [Header("Visual")]
    [SerializeField] private Button FullscreenButton = null;
    [SerializeField] private Button WindowedButton = null;
    [SerializeField] private Slider ContrastSlider = null;
    [SerializeField] private TextMeshProUGUI ContrastLabel = null;
    [SerializeField] private Slider BrightnessSlider = null;
    [SerializeField] private TextMeshProUGUI BrightnessLabel = null;
    [SerializeField] private Button VSyncOn = null;
    [SerializeField] private Button VSyncOff = null;
    [SerializeField] private Button QualityHigh = null;
    [SerializeField] private Button QualityMedium = null;
    [SerializeField] private Button QualityLow = null;

    [Header("Audio")]
    [SerializeField] private Slider MusicSlider = null;
    [SerializeField] private TextMeshProUGUI MusicLabel = null;
    [SerializeField] private Slider SFXSlider = null;
    [SerializeField] private TextMeshProUGUI SFXLabel = null;
    [SerializeField] private Slider DialogSlider = null;
    [SerializeField] private TextMeshProUGUI DialogLabel = null;
    [SerializeField] private Slider AmbienceSlider = null;
    [SerializeField] private TextMeshProUGUI AmbienceLabel = null;

    [Header("Font")]
    [SerializeField] private Button FancyFontButton = null;
    [SerializeField] private Button NormalFontButton = null;
    [SerializeField] private Button DyslexiaFontButton = null;

    [Header("Controls")]
    [SerializeField] private Button ClickDragOn = null;
    [SerializeField] private Button ClickDragOff = null;
    [SerializeField] private Button MotivatedMovementOn = null;
    [SerializeField] private Button MotivatedMovementOff = null;
    [SerializeField] private Button ControllerBorderMovement = null;
    [SerializeField] private Button ControllerJoystickMovement = null;

    // Update All Values from Settings
    private void OnEnable() {
        UpdateSettings();
    }

    public void UpdateSettings() {
        SetWindowed(Settings.Instance.isWindowed, false);

        ContrastSlider.value = Settings.Instance.contrast;
        ContrastLabel.text = Settings.Instance.contrast.ToString();
        BrightnessSlider.value = Settings.Instance.brightness;
        BrightnessLabel.text = Settings.Instance.brightness.ToString();

        SetVSync(Settings.Instance.vSync, false);
        SetQuality(Settings.Instance.graphicsQuality, false);

        MusicSlider.value = Settings.Instance.music;
        MusicLabel.text = Settings.Instance.music.ToString();
        SFXSlider.value = Settings.Instance.SFX;
        SFXLabel.text = Settings.Instance.SFX.ToString();
        DialogSlider.value = Settings.Instance.dialog;
        DialogLabel.text = Settings.Instance.dialog.ToString();
        AmbienceSlider.value = Settings.Instance.ambience;
        AmbienceLabel.text = Settings.Instance.ambience.ToString();

        SetFontStyle(Settings.Instance.textFont, false);
        SetClickAndDrag(Settings.Instance.useClickNDrag, false);
        SetMotivatedMovement(Settings.Instance.mouseMotivatedMovement, false);
        SetControllerMovement(Settings.Instance.controllerBorderMovement, false);
    }

    // Add Listeners to update settings when changed
    private void Start() {
        FullscreenButton.onClick.AddListener(SetFullscreen);
        WindowedButton.onClick.AddListener(SetWindowed);

        ContrastSlider.onValueChanged.AddListener(ChangeContrast);
        BrightnessSlider.onValueChanged.AddListener(ChangeBrightness);

        VSyncOn.onClick.AddListener(EnableVSync);
        VSyncOff.onClick.AddListener(DisableVSync);

        QualityHigh.onClick.AddListener(SetQualityHigh);
        QualityMedium.onClick.AddListener(SetQualityMedium);
        QualityLow.onClick.AddListener(SetQualityLow);

        MusicSlider.onValueChanged.AddListener(ChangeMusic);
        SFXSlider.onValueChanged.AddListener(ChangeSfx);
        DialogSlider.onValueChanged.AddListener(ChangeDialog);
        AmbienceSlider.onValueChanged.AddListener(ChangeAmbience);

        FancyFontButton.onClick.AddListener(SetFontFancy);
        NormalFontButton.onClick.AddListener(SetFontNormal);
        DyslexiaFontButton.onClick.AddListener(SetFontDyslexia);

        ClickDragOn.onClick.AddListener(EnableClickNDrag);
        ClickDragOff.onClick.AddListener(DisableClickNDrag);

        MotivatedMovementOn.onClick.AddListener(EnableMotivatedMovement);
        MotivatedMovementOff.onClick.AddListener(DisableMotivatedMovement);

        ControllerBorderMovement.onClick.AddListener(SetControllerBorderMovement);
        ControllerJoystickMovement.onClick.AddListener(SetControllerJoystickMovement);
    }

    // ---------------- Visual ----------------

    public void SetFullscreen() => SetWindowed(false);
    public void SetWindowed() => SetWindowed(true);

    public void SetWindowed(bool windowed, bool canSave = true) {
        Settings.Instance.isWindowed = windowed;
        FullscreenButton.interactable = windowed;
        WindowedButton.interactable = !windowed;
        if (canSave && SaveOnChange) SaveVisuals();
    }

    public void ChangeContrast(float value) {
        Settings.Instance.contrast = (int)value;
        ContrastLabel.text = ((int)value).ToString();
        if (SaveOnChange) SaveVisuals();
    }

    public void ChangeBrightness(float value) {
        Settings.Instance.brightness = (int)value;
        BrightnessLabel.text = ((int)value).ToString();
        if (SaveOnChange) SaveVisuals();
    }

    public void EnableVSync() => SetVSync(true);
    public void DisableVSync() => SetVSync(false);

    public void SetVSync(bool useVSync, bool canSave = true) {
        Settings.Instance.vSync = useVSync;
        VSyncOff.interactable = useVSync;
        VSyncOn.interactable = !useVSync;
        if (canSave && SaveOnChange) SaveVisuals();
    }

    public void SetQualityHigh() => SetQuality(0);
    public void SetQualityMedium() => SetQuality(1);
    public void SetQualityLow() => SetQuality(2);

    public void SetQuality(int graphicsQuality, bool canSave = true) {
        Settings.Instance.graphicsQuality = graphicsQuality;
        QualityHigh.interactable = graphicsQuality != 0;
        QualityMedium.interactable = graphicsQuality != 1;
        QualityLow.interactable = graphicsQuality != 2;
        if (canSave && SaveOnChange) SaveVisuals();
    }

    // ---------------- Audio ----------------

    public void ChangeMusic(float value) {
        Settings.Instance.music = (int)value;
        MusicLabel.text = ((int)value).ToString();
        if (SaveOnChange) SaveAudio();
    }

    public void ChangeSfx(float value) {
        Settings.Instance.SFX = (int)value;
        SFXLabel.text = ((int)value).ToString();
        if (SaveOnChange) SaveAudio();
    }

    public void ChangeDialog(float value) {
        Settings.Instance.dialog = (int)value;
        DialogLabel.text = ((int)value).ToString();
        if (SaveOnChange) SaveAudio();
    }

    public void ChangeAmbience(float value) {
        Settings.Instance.ambience = (int)value;
        AmbienceLabel.text = ((int)value).ToString();
        if (SaveOnChange) SaveAudio();
    }

    // ---------------- Font ----------------

    public void SetFontFancy() => SetFontStyle(0);
    public void SetFontNormal() => SetFontStyle(1);
    public void SetFontDyslexia() => SetFontStyle(2);

    public void SetFontStyle(int fontStyle, bool canSave = true) {
        Settings.Instance.textFont = fontStyle;
        FancyFontButton.interactable = fontStyle != 0;
        NormalFontButton.interactable = fontStyle != 1;
        DyslexiaFontButton.interactable = fontStyle != 2;
        if (canSave && SaveOnChange) SaveVisuals();
    }

    // -------------- Movement --------------

    public void EnableClickNDrag() => SetClickAndDrag(true);
    public void DisableClickNDrag() => SetClickAndDrag(false);

    public void SetClickAndDrag(bool useClickNDrag, bool canSave = true) {
        Settings.Instance.useClickNDrag = useClickNDrag;
        ClickDragOff.interactable = useClickNDrag;
        ClickDragOn.interactable = !useClickNDrag;
        if (canSave && SaveOnChange) SaveControls();
    }

    public void EnableMotivatedMovement() => SetMotivatedMovement(true);
    public void DisableMotivatedMovement() => SetMotivatedMovement(false);

    public void SetMotivatedMovement(bool useMouseMotivated, bool canSave = true) {
        Settings.Instance.mouseMotivatedMovement = useMouseMotivated;
        MotivatedMovementOff.interactable = useMouseMotivated;
        MotivatedMovementOn.interactable = !useMouseMotivated;
        if (canSave && SaveOnChange) SaveControls();
    }

    public void SetControllerBorderMovement() => SetControllerMovement(true);
    public void SetControllerJoystickMovement() => SetControllerMovement(false);

    public void SetControllerMovement(bool border, bool canSave = true) {
        Settings.Instance.controllerBorderMovement = border;
        ControllerJoystickMovement.interactable = border;
        ControllerBorderMovement.interactable = !border;
        if (canSave && SaveOnChange) SaveControls();
    }


    // ----------- Save Functions -----------

    public void SaveVisuals() {
        Settings.Instance.SaveVisualSettings();
    }

    public void SaveAudio() {
        Settings.Instance.SaveAudioSettings();
    }

    public void SaveControls() {
        Settings.Instance.SaveControlSettings();
    }
}