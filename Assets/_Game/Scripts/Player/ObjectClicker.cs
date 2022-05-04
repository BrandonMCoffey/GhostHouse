using UnityEngine;
using UnityEngine.EventSystems;

namespace Mechanics.Player
{
    public class ObjectClicker : MonoBehaviour
    {
        private CameraController _controller;
        private IInteractable _previousInteractable;
        private Vector3 _previousPosition;

        #region Properties

        // Input
        private static bool LeftClick => Input.GetMouseButtonDown(0);
        private static bool RightClick => Input.GetMouseButtonDown(1);

        // Checks if mouse is over UI
        public static bool IsMouseOverUi {
            get {
                var events = EventSystem.current;
                return events != null && events.IsPointerOverGameObject();
            }
        }

        #endregion

        #region Unity Functions

        private void OnEnable() {
            UserInput.Interact += OnUserClick;
        }

        private void OnDisable() {
            UserInput.Interact -= OnUserClick;
        }

        private void Start() {
            _controller = CameraController.Singleton;
        }

        private void Update() {
            RaycastCheck();
        }

        #endregion

        #region Hovering and Clicking

        private void RaycastCheck() {
            // Ignore raycast if mouse is over UI
            if (IsMouseOverUi) {
                ResetHover();
                return;
            }

            // Get mouse position and raycast
            Ray ray = _controller.Camera.ScreenPointToRay(_controller.MousePos);

            // Raycast and hit
            if (Physics.Raycast(ray, out var hit)) {
                // Get Interactable
                var interactable = hit.transform.GetComponent<IInteractable>();
                if (interactable == null && hit.transform.parent != null) {
                    interactable = hit.transform.parent.GetComponent<IInteractable>();
                }
                _previousPosition = hit.point;
                OnHover(interactable);
            }
            // Raycast failed to hit anything
            else {
                ResetHover();
            }
        }

        private void OnHover(IInteractable interactable) {
            if (interactable != _previousInteractable) {
                ResetHover(interactable);
            }
        }

        private void ResetHover(IInteractable newInteractable = null) {
            // Attempt to stop hovering previous object
            _previousInteractable?.OnHoverExit();

            // Attempt to hover new object
            newInteractable?.OnHoverEnter();

            // Set new object as previous object
            _previousInteractable = newInteractable;
        }

        private void OnUserClick() {
            OnClick(_previousInteractable, _previousPosition);
        }

        private static void OnClick(IInteractable interactable, Vector3 position) {
            interactable?.OnLeftClick(position);
        }

        #endregion
    }
}