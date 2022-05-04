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

    [Header("WASD Movement")]
    [SerializeField] private bool _wasdMovement = true;
    [SerializeField] private float _wasdSpeed = 10f;
    [SerializeField] private bool _wasdSprint = false;
    [SerializeField, Range(0, 5)] private float _wasdSprintMultiplier = 2f;

    [Header("Mouse Motivated Movement")]
    [SerializeField] private bool _mouseMotivatedMovement = false;
    [SerializeField] private float _mouseMotivatedBorderMin = 50f;
    [SerializeField] private float _mouseMotivatedBorderMax = 200f;
    [SerializeField] private float _mouseMotivatedSpeed = 25f;
    [SerializeField] private bool _mouseMotivatedSprint = false;
    [SerializeField, Range(0, 5)] private float _mouseMotivatedSprintMultiplier = 1.4f;

    [Header("Click and Drag Movement")]
    [SerializeField] private bool _clickAndDrag = false;
    [SerializeField, Range(0, 1)] private float _clickDragSmooth = 0.5f;
    [SerializeField] private LayerMask _groundLayer = 0;
    [SerializeField, ReadOnly] private bool _dragging;

    [Header("Sprinting")]
    [SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;
    [SerializeField, ReadOnly] private bool _sprintKeyHeld = false;

    [Header("Smooth Camera Movement")]
    [SerializeField] private bool _smoothCameraMovement = true;
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Camera Bounds")]
    [SerializeField] private float _maxXValue = 44f;
    [SerializeField] private float _minXValue = -37f;
    [SerializeField] private float _maxZValue = 35f;
    [SerializeField] private float _minZValue = -40f;

    private DialogueRunner _dialogueRunner;

    // Input Values
    private float _horizontal;
    private float _vertical;
    private float _mouseWheel;

    // Input Lock Variables
    public static bool Interacting { get; set; }
    public static bool FadeToBlackPlaying { get; set; }
    public static bool IsTransitioning { get; set; }
    private static bool GamePaused => PauseMenu.Singleton.IsPaused;
    private bool DialoguePlaying => _dialogueRunner.IsDialogueRunning;
    public static bool Dragging => Singleton._dragging;

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

    private void Awake() {
        if (Singleton == null) {
            Singleton = this;
        }
        else {
            Destroy(gameObject);
        }

        _dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    private void OnEnable() {
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
        if (_clickAndDrag) {
            HandleClickAndDragMovement();
        }
        if (!_dragging) {
            if (_wasdMovement) {
                HandleWasdMovement();
            }
            if (_mouseMotivatedMovement) {
                HandleMouseMotivatedMovement();
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
        _mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _sprintKeyHeld = Input.GetKey(_sprintKey);
    }

    private void HandleCameraZoom() {
        if (_cam == null) return;
        var zoomDelta = _mouseWheel * _camZoomSpeed;
        _cam.orthographicSize = Mathf.Clamp(zoomDelta, _camZoomInMax, _camZoomOutMax);
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

    private void HandleMouseMotivatedMovement() {
        Vector3 rightMovement = Vector3.zero;
        Vector3 upMovement = Vector3.zero;

        var mousePos = Input.mousePosition;
        if (mousePos.x >= Screen.width - _mouseMotivatedBorderMax) {
            // Right border, move right
            float inside = Screen.width - _mouseMotivatedBorderMax;
            float outside = Screen.width - _mouseMotivatedBorderMin;
            float delta = Map01(mousePos.x, inside, outside);
            rightMovement = _right * _mouseMotivatedSpeed * delta * Time.deltaTime;
        }
        else if (mousePos.x <= _mouseMotivatedBorderMax) {
            // Left border, move left
            float inside = _mouseMotivatedBorderMax;
            float outside = _mouseMotivatedBorderMin;
            float delta = Map01(mousePos.x, inside, outside);
            rightMovement = -_right * _mouseMotivatedSpeed * delta * Time.deltaTime;
        }
        if (mousePos.y >= Screen.height - _mouseMotivatedBorderMax) {
            // Top border, move up
            float inside = Screen.height - _mouseMotivatedBorderMax;
            float outside = Screen.height - _mouseMotivatedBorderMin;
            float delta = Map01(mousePos.y, inside, outside);
            upMovement = _forward * _mouseMotivatedSpeed * delta * Time.deltaTime;
        }
        else if (mousePos.y <= _mouseMotivatedBorderMax) {
            // Bottom border, move down
            float inside = _mouseMotivatedBorderMax;
            float outside = _mouseMotivatedBorderMin;
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
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _groundLayer)) {
                _dragging = true;
                _clickDragStart = hit.point;
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            _dragging = false;
        }
        else if (_dragging && Input.GetMouseButton(0)) {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
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

    private void InteractStarted() {
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