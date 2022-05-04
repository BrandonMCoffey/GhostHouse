using Mechanics.Level_Mechanics;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility.ReadOnly;
using Yarn.Unity;

public class CameraController : MonoBehaviour
{
    public static CameraController Singleton { get; private set; }

    [SerializeField] private Camera _cam;

    [Header("Camera Zoom")]
    [SerializeField] private bool _camZoom = false;
    [SerializeField] private float _camZoomSpeed = 5f;
    [SerializeField] private float _camZoomInMax = 0.7f;
    [SerializeField] private float _camZoomOutMax = 6.37f;

    [Header("Controller Movement")]
    [SerializeField] private float _controllerSpeed = 400f;
    [SerializeField] private bool _controllerSprint = true;
    [SerializeField, Range(0, 5)] private float _controllerSprintMultiplier = 2f;
    [SerializeField] private bool _controllerBorder = true;
    [SerializeField, Range(0, 5)] private float _controllerBorderMultiplier = 1.5f;
    [SerializeField] private bool _controllerJoystick = true;
    [SerializeField] private float _controllerJoystickSpeed = 20f;
    [SerializeField] private bool _controllerJoystickSprint = true;
    [SerializeField] private float _controllerJoystickSprintMultiplier = 1.5f;

    [Header("WASD Movement")]
    [SerializeField] private bool _wasdMovement = true;
    [SerializeField] private float _wasdSpeed = 20f;
    [SerializeField] private bool _wasdSprint = false;
    [SerializeField, Range(0, 5)] private float _wasdSprintMultiplier = 1.5f;
    [SerializeField, ReadOnly] private bool _sprintKeyHeld;

    [Header("Mouse Motivated Movement")]
    [SerializeField] private bool _mouseMotivatedMovement;
    [SerializeField] private float _mouseMotivatedBorderMin = 50f;
    [SerializeField] private float _mouseMotivatedBorderMax = 250f;
    [SerializeField] private float _mouseMotivatedSpeed = 25f;
    [SerializeField] private bool _mouseMotivatedSprint = false;
    [SerializeField, Range(0, 5)] private float _mouseMotivatedSprintMultiplier = 1.4f;

    [Header("Click and Drag Movement")]
    [SerializeField] private bool _clickAndDrag;
    [SerializeField, Range(0, 1)] private float _clickDragSmooth = 0.5f;
    [SerializeField] private LayerMask _groundLayer = 0;
    [SerializeField, ReadOnly] private bool _dragging;

    [Header("Smooth Camera Movement")]
    [SerializeField] private bool _smoothCameraMovement = true;
    [SerializeField] private float _smoothSpeed = 20f;
    [SerializeField] private float _smoothStopSpeed = 30f;

    [Header("Camera Bounds")]
    [SerializeField] private float _maxXValue = 44f;
    [SerializeField] private float _minXValue = -37f;
    [SerializeField] private float _maxZValue = 35f;
    [SerializeField] private float _minZValue = -40f;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private float _horizontal;
    [SerializeField, ReadOnly] private float _vertical;
    [SerializeField, ReadOnly] private float _mouseWheel;
    [SerializeField, ReadOnly] private Vector3 _mousePos;
    [SerializeField, ReadOnly] private Vector2 _controllerPos;

    private DialogueRunner _dialogueRunner;
    private ControllerVisuals _controllerVisuals;

    public Camera Camera => _cam;
    public static bool UsingController { get; private set; }
    public static bool Dragging => Singleton._dragging;
    public Vector3 MousePos => UsingController ? (Vector3)_controllerPos : UserInput.MousePosition;
    private static Vector2 ScreenCenter => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    // Input Lock Variables
    public static bool Interacting { get; set; }
    public static bool FadeToBlackPlaying { get; set; }
    public static bool IsTransitioning { get; set; }
    private static bool GamePaused => PauseMenu.IsPaused;
    private bool DialoguePlaying => _dialogueRunner.IsDialogueRunning;

    // Camera Transform Data
    private Vector3 _prevGoalPosition;
    private Vector3 _goalPosition;
    private Vector3 _forward;
    private Vector3 _right;

    // Camera Lerp to Position
    private Vector3 _clickDragStart;
    private bool _lerpToPosition;
    private Vector3 _finalLerpPosition;
    private float _finalLerpTime;
    private float _finalLerpEndTime;

    private void Awake() {
        if (Singleton == null) {
            Singleton = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        if (_dialogueRunner == null) _dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (_controllerVisuals == null) _controllerVisuals = FindObjectOfType<ControllerVisuals>();

        ModalWindowController.OnInteractStart += InteractStarted;
        ModalWindowController.OnInteractEnd += InteractEnded;
    }

    private void OnDisable() {
        ModalWindowController.OnInteractStart -= InteractStarted;
        ModalWindowController.OnInteractEnd -= InteractEnded;
    }

    private void Start() {
        if (_cam == null) _cam = Camera.main;
        if (_cam != null) {
            _forward = _cam.transform.forward;
            _right = _cam.transform.right;
        }

        _forward.y = 0f;
        _forward = Vector3.Normalize(_forward);
        _right = Vector3.Normalize(_right);
        _goalPosition = transform.position;

        if (UsingController) {
            _controllerPos = ScreenCenter;
            if (_controllerVisuals != null) {
                _controllerVisuals.SetCursorPosition(ScreenCenter);
                _controllerVisuals.SetIndependent(false);
            }
        }
    }

    private void Update() {
        if (_lerpToPosition) {
            LerpToPosition();
            return;
        }
        CheckInputType();
        GatherInput();
        if (UsingController) {
            HandleControllerMovement();
        }
        if (Interacting || GamePaused || FadeToBlackPlaying || DialoguePlaying || IsTransitioning) {
            return;
        }
        if (_camZoom) {
            HandleCameraZoom();
        }
        if (UsingController) {
            if (_controllerBorder) {
                HandleBorderMotivatedMovement(_controllerPos, _controllerBorderMultiplier);
            }
            if (_controllerJoystick) {
                HandleControllerJoystickMovement();
            }
        }
        else {
            if (_clickAndDrag) {
                HandleClickAndDragMovement();
            }
            if (!_dragging) {
                if (_wasdMovement) {
                    HandleWasdMovement();
                }
                if (_mouseMotivatedMovement) {
                    HandleBorderMotivatedMovement(_mousePos);
                }
            }
        }
        HandleGoalMovement();
    }

    private void LateUpdate() {
        HandleMovement();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_goalPosition, 0.25f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }

    private void LerpToPosition() {
        _finalLerpTime += Time.deltaTime;
        float movementPercentage = _finalLerpTime / _finalLerpEndTime;
        _goalPosition = Vector3.Lerp(_goalPosition, _finalLerpPosition, movementPercentage);
        if (UsingController) {
            _controllerPos = Vector2.Lerp(_controllerPos, ScreenCenter, movementPercentage);
        }

        if (_goalPosition == _finalLerpPosition) {
            _finalLerpTime = 0f;
            _lerpToPosition = false;
        }
    }

    private void CheckInputType() {
        var mousePos = UserInput.MousePosition;
        bool mouseMoved = mousePos != _mousePos;
        _mousePos = mousePos;

        if (UsingController) {
            var horz = UserInput.Horizontal;
            var vert = UserInput.Vertical;

            if (horz + vert != 0 || mouseMoved) {
                ToggleController(false);
            }
        }
        else {
            var horz = UserInput.HorizontalController + (_controllerJoystick ? UserInput.HorizontalController2 : 0);
            var vert = UserInput.VerticalController + (_controllerJoystick ? UserInput.VerticalController2 : 0);

            if (horz + vert != 0) {
                ToggleController(true);
            }
        }
    }

    private void ToggleController(bool controller) {
        if (_controllerVisuals != null) {
            _controllerVisuals.ToggleCustomCursor(controller);
            if (controller) {
                _controllerPos = _mousePos;
                _controllerVisuals.SetCursorPosition(_mousePos);
            }
        }
        UsingController = controller;
    }

    private void GatherInput() {
        _mouseWheel = UserInput.MouseScrollWheel;
        _sprintKeyHeld = UserInput.Sprinting;
        if (UsingController) {
            _horizontal = UserInput.HorizontalController;
            _vertical = UserInput.VerticalController;
        }
        else {
            _horizontal = UserInput.Horizontal;
            _vertical = UserInput.Vertical;
        }
    }

    private void HandleCameraZoom() {
        if (_cam == null) return;
        var zoomDelta = _mouseWheel * _camZoomSpeed;
        _cam.orthographicSize = Mathf.Clamp(zoomDelta, _camZoomInMax, _camZoomOutMax);
    }

    private void HandleControllerMovement() {
        float rightMovement = _horizontal * _controllerSpeed * Time.deltaTime;
        float upMovement = _vertical * _controllerSpeed * Time.deltaTime;

        if (_sprintKeyHeld && _controllerSprint) {
            rightMovement *= _controllerSprintMultiplier;
            upMovement *= _controllerSprintMultiplier;
        }

        _controllerPos.x += rightMovement;
        _controllerPos.y += upMovement;

        _controllerPos.x = Mathf.Clamp(_controllerPos.x, 0, Screen.width);
        _controllerPos.y = Mathf.Clamp(_controllerPos.y, 0, Screen.height);

        if (_controllerVisuals != null) _controllerVisuals.SetCursorPosition(_controllerPos);
    }

    private void HandleControllerJoystickMovement() {
        Vector2 movement = new Vector2(UserInput.HorizontalController2, UserInput.VerticalController2);
        if (movement.magnitude > 1) {
            movement = movement.normalized;
        }
        Vector3 rightMovement = movement.x * _right * _controllerJoystickSpeed * Time.deltaTime;
        Vector3 upMovement = movement.y * _forward * _controllerJoystickSpeed * Time.deltaTime;

        if (_controllerJoystickSprint && _sprintKeyHeld) {
            rightMovement *= _controllerJoystickSprintMultiplier;
            upMovement *= _controllerJoystickSprintMultiplier;
        }

        _goalPosition += rightMovement + upMovement;
    }

    private void HandleWasdMovement() {
        Vector2 movement = new Vector2(_horizontal, _vertical);
        if (movement.magnitude > 1) {
            movement = movement.normalized;
        }
        Vector3 rightMovement = movement.x * _right * _wasdSpeed * Time.deltaTime;
        Vector3 upMovement = movement.y * _forward * _wasdSpeed * Time.deltaTime;

        if (_wasdSprint && _sprintKeyHeld) {
            rightMovement *= _wasdSprintMultiplier;
            upMovement *= _wasdSprintMultiplier;
        }

        _goalPosition += rightMovement + upMovement;
    }

    private static float Map01(float value, float min, float max) {
        return Mathf.Clamp01((value - min) / (max - min));
    }

    private void HandleBorderMotivatedMovement(Vector2 mousePos, float multiplier = 1) {
        Vector3 rightMovement = Vector3.zero;
        Vector3 upMovement = Vector3.zero;

        float borderMax = _mouseMotivatedBorderMax * multiplier;
        float borderMin = _mouseMotivatedBorderMin * multiplier;

        if (mousePos.x >= Screen.width - borderMax) {
            // Right border, move right
            float inside = Screen.width - borderMax;
            float outside = Screen.width - borderMin;
            float delta = Map01(mousePos.x, inside, outside);
            rightMovement = _right * _mouseMotivatedSpeed * delta * Time.deltaTime;
        }
        else if (mousePos.x <= borderMax) {
            // Left border, move left
            float inside = borderMax;
            float outside = borderMin;
            float delta = Map01(mousePos.x, inside, outside);
            rightMovement = -_right * _mouseMotivatedSpeed * delta * Time.deltaTime;
        }
        if (mousePos.y >= Screen.height - borderMax) {
            // Top border, move up
            float inside = Screen.height - borderMax;
            float outside = Screen.height - borderMin;
            float delta = Map01(mousePos.y, inside, outside);
            upMovement = _forward * _mouseMotivatedSpeed * delta * Time.deltaTime;
        }
        else if (mousePos.y <= borderMax) {
            // Bottom border, move down
            float inside = borderMax;
            float outside = borderMin;
            float delta = Map01(mousePos.y, inside, outside);
            upMovement = -_forward * _mouseMotivatedSpeed * delta * Time.deltaTime;
        }

        if (_mouseMotivatedSprint && _sprintKeyHeld) {
            rightMovement *= _mouseMotivatedSprintMultiplier;
            upMovement *= _mouseMotivatedSprintMultiplier;
        }

        _goalPosition += rightMovement + upMovement;
    }

    private void HandleClickAndDragMovement() {
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUi) {
            Ray ray = _cam.ScreenPointToRay(_mousePos);
            if (Physics.Raycast(ray, out var hit)) {
                var interactable = hit.transform.parent.GetComponent<StoryInteractable>();
                if (interactable != null) return;
                _dragging = true;
                _clickDragStart = hit.point;
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            _dragging = false;
        }
        else if (_dragging && Input.GetMouseButton(0)) {
            Ray ray = _cam.ScreenPointToRay(_mousePos);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _groundLayer)) {
                Vector3 diff = _clickDragStart - Vector3.Lerp(_clickDragStart, hit.point, _clickDragSmooth);
                diff.y = 0;
                _goalPosition += diff;
            }
        }
    }

    private void HandleGoalMovement() {
        _goalPosition = CameraBounds(_goalPosition);
        if (_prevGoalPosition == _goalPosition) {
            _goalPosition = Vector3.Lerp(_goalPosition, transform.position, _smoothStopSpeed * Time.deltaTime);
        }
        _prevGoalPosition = _goalPosition;
    }

    private Vector3 CameraBounds(Vector3 location) {
        var x = Mathf.Clamp(location.x, _minXValue, _maxXValue);
        var z = Mathf.Clamp(location.z, _minZValue, _maxZValue);
        return new Vector3(x, 0, z);
    }

    private void HandleMovement() {
        var smoothed = Vector3.Lerp(transform.position, _goalPosition, _smoothSpeed * Time.deltaTime);
        transform.position = _smoothCameraMovement && !_dragging ? smoothed : _goalPosition;
    }

    private static void InteractStarted() {
        Interacting = true;
    }

    private void InteractEnded() {
        Interacting = false;
        _finalLerpTime = 0f;
    }

    public void MoveToPosition(Vector3 finalPosition, float movementTime) {
        _finalLerpPosition = CameraBounds(finalPosition);
        _lerpToPosition = true;
        _finalLerpEndTime = movementTime;
    }

    public void SetClickDragEnabled(bool enable) {
        _clickAndDrag = enable;
    }

    public void SetMouseMotivatedEnabled(bool enable) {
        _mouseMotivatedMovement = enable;
    }

    public void SetControllerMovement(bool border) {
        _controllerBorder = border;
        _controllerJoystick = !border;
    }

    public static bool IsMouseOverUi {
        get {
            var events = EventSystem.current;
            return events != null && events.IsPointerOverGameObject();
        }
    }
}