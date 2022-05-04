using Mechanics.Level_Mechanics;
using UI;
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
    [SerializeField] private float _controllerSpeed = 100f;
    [SerializeField] private bool _controllerSprint = true;
    [SerializeField, Range(0, 5)] private float _controllerSprintMultiplier = 2f;
    [SerializeField, Range(0, 5)] private float _controllerBorderMultiplier = 2f;

    [Header("WASD Movement")]
    [SerializeField] private bool _wasdMovement = true;
    [SerializeField] private float _wasdSpeed = 10f;
    [SerializeField] private bool _wasdSprint = false;
    [SerializeField, Range(0, 5)] private float _wasdSprintMultiplier = 2f;
    [SerializeField, ReadOnly] private bool _sprintKeyHeld;

    [Header("Mouse Motivated Movement")]
    [SerializeField] private bool _mouseMotivatedMovement = false;
    [SerializeField] private float _mouseMotivatedBorderMin = 50f;
    [SerializeField] private float _mouseMotivatedBorderMax = 200f;
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
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Camera Bounds")]
    [SerializeField] private float _maxXValue = 44f;
    [SerializeField] private float _minXValue = -37f;
    [SerializeField] private float _maxZValue = 35f;
    [SerializeField] private float _minZValue = -40f;

    private DialogueRunner _dialogueRunner;
    private PlayerHUD _playerHud;

    // Input Values
    private float _horizontal;
    private float _vertical;
    private float _mouseWheel;
    private Vector3 _mousePos;

    public Camera Camera => _cam;
    public bool UsingController { get; private set; }
    public static bool Dragging => Singleton._dragging;
    public Vector3 MousePos => UsingController ? (Vector3)_controllerPos : UserInput.MousePosition;

    // Input Lock Variables
    public static bool Interacting { get; set; }
    public static bool FadeToBlackPlaying { get; set; }
    public static bool IsTransitioning { get; set; }
    private static bool GamePaused => PauseMenu.Singleton.IsPaused;
    private bool DialoguePlaying => _dialogueRunner.IsDialogueRunning;

    // Camera Transform Data
    private Vector3 _goalPosition;
    private Vector3 _forward;
    private Vector3 _right;

    // Camera Lerp to Position
    private bool _lerpToPosition;
    private Vector3 _finalLerpPosition;
    private float _finalLerpTime;
    private float _finalLerpEndTime;

    // Movement variables
    private Vector3 _clickDragStart;
    private Vector2 _controllerPos;

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
        if (_playerHud == null) _playerHud = FindObjectOfType<PlayerHUD>();

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
    }

    private void Update() {
        if (_lerpToPosition) {
            LerpToPosition();
            return;
        }
        if (Interacting || GamePaused || FadeToBlackPlaying || DialoguePlaying || IsTransitioning) {
            return;
        }
        GatherInput();
        if (_camZoom) {
            HandleCameraZoom();
        }
        if (UsingController) {
            HandleControllerMovement();
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
        _goalPosition = CameraBounds(_goalPosition);
    }

    private void LateUpdate() {
        var smoothed = Vector3.Lerp(transform.position, _goalPosition, _smoothSpeed * Time.deltaTime);
        transform.position = _smoothCameraMovement && !_dragging ? smoothed : _goalPosition;
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

        if (_goalPosition == _finalLerpPosition) {
            _finalLerpTime = 0f;
            _lerpToPosition = false;
        }
    }

    private void GatherInput() {
        _mouseWheel = UserInput.MouseScrollWheel;
        _sprintKeyHeld = UserInput.Sprinting;
        var mousePos = UserInput.MousePosition;
        bool mouseMoved = mousePos != _mousePos;
        _mousePos = mousePos;

        if (UsingController) {
            _horizontal = UserInput.Horizontal;
            _vertical = UserInput.Vertical;

            if (_horizontal + _vertical != 0 || mouseMoved) {
                // Now using keyboard / mouse
                ToggleController(false);
                return;
            }
            _horizontal = UserInput.HorizontalController;
            _vertical = UserInput.VerticalController;
        }
        else {
            _horizontal = UserInput.HorizontalController;
            _vertical = UserInput.VerticalController;

            if (_horizontal + _vertical != 0) {
                // Now using controller
                ToggleController(true);
                return;
            }
            _horizontal = UserInput.Horizontal;
            _vertical = UserInput.Vertical;
        }
    }

    private void ToggleController(bool controller) {
        Cursor.visible = !controller;
        if (_playerHud != null) {
            _playerHud.ToggleCustomCursor(controller);
            if (controller) {
                _controllerPos = _mousePos;
                _playerHud.SetCursorPosition(_mousePos);
            }
        }
        UsingController = controller;
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

        HandleBorderMotivatedMovement(_controllerPos, _controllerBorderMultiplier);

        _playerHud.SetCursorPosition(_controllerPos);
    }

    private void HandleWasdMovement() {
        Vector3 rightMovement = _horizontal * _right * _wasdSpeed * Time.deltaTime;
        Vector3 upMovement = _vertical * _forward * _wasdSpeed * Time.deltaTime;

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

    private Vector3 CameraBounds(Vector3 location) {
        var x = Mathf.Clamp(location.x, _minXValue, _maxXValue);
        var z = Mathf.Clamp(location.z, _minZValue, _maxZValue);
        return new Vector3(x, 0, z);
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

    public static bool IsMouseOverUi {
        get {
            var events = EventSystem.current;
            return events != null && events.IsPointerOverGameObject();
        }
    }
}