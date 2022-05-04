using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class GraphicsController : MonoBehaviour
{
    public static GraphicsController Instance { get; private set; }

    private static FullScreenMode _screenMode = FullScreenMode.FullScreenWindow;
    public static FullScreenMode ScreenMode
    {
        get => _screenMode;
        set
        {
            _screenMode = value;
            UpdateScreenMode();
        }
    }

    private static float _exposure;
    public static float Exposure
    {
        get => _exposure;
        set
        {
            _exposure = value;
            Instance?.SetExposure();
        }
    }

    private static float _contrast;
    public static float Contrast
    {
        get => _contrast;
        set
        {
            _contrast = value;
            Instance?.SetContrast();
        }
    }

    [SerializeField] private Vector2 _exposureBounds = new Vector2(-100f, 100f);

    [SerializeField] private Vector2 _contrastBounds = new Vector2(-100, 100f);

    private ColorAdjustments _colorAdjustments;
    private float _initExposure, _initContrast;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        VolumeProfile profile = GetComponent<Volume>()?.sharedProfile;
        if (profile == null) return;

        // ensure we have a ColorAdjustments component
        if (!profile.TryGet(out _colorAdjustments))
        {
            _colorAdjustments = profile.Add<ColorAdjustments>();
        }

        // enable necessary settings
        _colorAdjustments.postExposure.overrideState = true;
        _colorAdjustments.contrast.overrideState = true;

        _initExposure = _colorAdjustments.postExposure.value;
        _initContrast = _colorAdjustments.contrast.value;

        SetExposure();
        SetContrast();
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        Instance = null;

        if (_colorAdjustments != null) {
            _colorAdjustments.postExposure.value = _initExposure;
            _colorAdjustments.contrast.value = _initContrast;
        }
    }

    public static void UpdateScreenMode()
    {
        Screen.fullScreenMode = _screenMode;
    }

    private void SetContrast()
    {
        if (_colorAdjustments == null) return;
        _colorAdjustments.contrast.value = Mathf.Clamp(_contrast, _contrastBounds.x, _contrastBounds.y);
    }

    private void SetExposure()
    {
        if (_colorAdjustments == null) return;
        _colorAdjustments.postExposure.value = Mathf.Clamp(_exposure, _exposureBounds.x, _exposureBounds.y);
    }
}