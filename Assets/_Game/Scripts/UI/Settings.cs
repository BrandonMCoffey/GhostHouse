﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Utility.Buttons;
using Utility.Audio.Managers;

//Settings menus change settings values
public class Settings : MonoBehaviour
{
    //Singleton pattern
    private static Settings _instanceReference;
    public static Settings Instance {
        get {
            if (_instanceReference == null) {
                _instanceReference = FindObjectOfType<Settings>();
            }
            return _instanceReference;
        }
    }

    //Interact Button
    public bool leftClickInteract = true;

    //Camera Movement
    public bool useWASD = true;
    public bool useArrowKeys = true;
    public bool useClickNDrag;
    public int dragSpeed = 75;

    //Audio Settings
    public int music = 75;
    public int SFX = 75;
    public int dialog = 75;
    public int ambience = 75;

    //Visual Settings
    public bool isWindowed;
    public int contrast;
    [SerializeField] private int contrastScale = 10;
    public int brightness;
    [SerializeField] private int brightnessScale = 10;
    public bool vSync;
    public bool largeGUIFont;
    public bool largeTextFont;
    public int graphicsQuality; // 0 = Highest, 1 = Medium, 2 = Lowest

    //0 - Fancy, 1 - Normal, 2 - Dyslexia Friendly
    [Range(0, 2)]
    public int textFont = 2;

    /* Lazy load the Camera Controller
    private IsometricCameraController cameraController;
    private IsometricCameraController CameraController
    {
        get
        {
            if(cameraController == null) cameraController = FindObjectOfType<IsometricCameraController>();
            return cameraController;
        }
    }*/

    // Reference to AudioMixerController to control volume levels
    private AudioMixerController audioMixerController;

    private void Awake() {
        if (_instanceReference == null) {
            _instanceReference = this;
            audioMixerController = GetComponent<AudioMixerController>();
            DontDestroyOnLoad(gameObject);
        }
        else if (_instanceReference != this) {
            Destroy(gameObject);
        }
    }

    //Load all settings when game starts
    private void Start() {
        //Debug.Log(Application.persistentDataPath);
        //Debug.Log(DataManager.Instance.settingsLeftClickInteract);

        SceneManager.activeSceneChanged += (Scene before, Scene after) => SaveAllSettings();

        LoadSettings();
        SetControlSettings();
        SetAudioSettings();
        SetVisualSettings();
    }

    [Button(Spacing = 25, Mode = ButtonMode.NotPlaying)]
    public void LoadSettings() {
        // This should be called each time the settings menu is opened

        leftClickInteract = DataManager.Instance.settingsLeftClickInteract;
        useWASD = DataManager.Instance.settingsCameraWASD;
        useArrowKeys = DataManager.Instance.settingsCameraArrowKeys;
        useClickNDrag = DataManager.Instance.settingsClickDrag;
        dragSpeed = DataManager.Instance.settingsSensitivity;
        music = DataManager.Instance.settingsMusicVolume;
        SFX = DataManager.Instance.settingsSFXVolume;
        dialog = DataManager.Instance.settingsDialogueVolume;
        ambience = DataManager.Instance.settingsAmbienceVolume;
        isWindowed = DataManager.Instance.settingsWindowMode;
        contrast = DataManager.Instance.settingsContrast;
        brightness = DataManager.Instance.settingsBrightness;
        largeGUIFont = DataManager.Instance.settingsLargeGUI;
        largeTextFont = DataManager.Instance.settingsLargeText;
        textFont = DataManager.Instance.settingsTextFont;
        vSync = DataManager.Instance.settingsVSync;
        graphicsQuality = DataManager.Instance.settingsGraphicsQuality;
    }

    [Button(Spacing = 20, Mode = ButtonMode.NotPlaying)]
    public void SaveAllSettings() {
        SaveControlSettings();
        SaveAudioSettings();
        SaveVisualSettings();
    }

    [Button(Spacing = 10, Mode = ButtonMode.NotPlaying)]
    public void SaveControlSettings() {
        DataManager.Instance.SaveControlSettings(leftClickInteract, useWASD, useArrowKeys, useClickNDrag, dragSpeed);
        SetControlSettings();
    }

    [Button(Mode = ButtonMode.NotPlaying)]
    public void SaveAudioSettings() {
        DataManager.Instance.SaveAudioSettings(music, SFX, dialog, ambience);
        SetAudioSettings();
    }

    [Button(Mode = ButtonMode.NotPlaying)]
    public void SaveVisualSettings() {
        DataManager.Instance.SaveVisualSettings(isWindowed, contrast, brightness, largeGUIFont, largeTextFont, textFont, vSync, graphicsQuality);
        SetVisualSettings();
    }

    // Update the camera controller with the new settings
    private void SetControlSettings()
    {
        // Set Control settings on camera controller
        //if (CameraController == null) {
            //Debug.LogWarning("No Camera Controller", gameObject);
            //return;
        //}
        //CameraController._enableWASDMovement = useWASD;
        if (CameraController.Singleton != null) {
            CameraController.Singleton.SetClickDragEnabled(useClickNDrag);
        }
    }

    // Update audio mixer controller with audio values
    private void SetAudioSettings()
    {
        if (audioMixerController == null) {
            //Debug.LogWarning("No Audio Mixer Controller", gameObject);
            return;
        }
        // Assuming 0 to 100 instead of 0 to 1
        audioMixerController.SetMusicVolume(music * 0.01f);
        audioMixerController.SetSfxVolume(SFX * 0.01f);
        audioMixerController.SetDialogueVolume(dialog * 0.01f);
        audioMixerController.SetAmbienceVolume(ambience * 0.01f);
    }

    // Update visual settings in the font manager and elsewhere
    private void SetVisualSettings()
    {
        // set font
        FontManager fontManager = FontManager.Instance;
        fontManager.UpdateAllText((FontMode)textFont);

        // set post-processing volume
        GraphicsController.ScreenMode = isWindowed ? FullScreenMode.FullScreenWindow : FullScreenMode.ExclusiveFullScreen;
        GraphicsController.Exposure = brightnessScale * brightness;
        GraphicsController.Contrast = contrastScale * contrast;

        QualitySettings.vSyncCount = vSync ? 1 : 0;
        QualitySettings.SetQualityLevel(graphicsQuality);
    }
}