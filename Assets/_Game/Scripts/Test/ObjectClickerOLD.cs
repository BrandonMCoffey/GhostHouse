﻿/*
using UnityEngine;

public class ObjectClickerOLD : MonoBehaviour
{
    // So this is the class that manages the mouse input. I originally wanted to name it as: MouseInput, but at the time I was making this
    // it was like 3 am, so my brain couldn't come up with a good name. Basically, this class casts raycasts (I think a total of 2 depending on whether
    // the player is hovering or clicking) and calls the methods in the IInteractable Interface accordingly.


    // This is the distance from the camera that the mouse can click. If an object is over 100 units away, the mouse click raycast will not
    // register its existance. Usually any object will not be outside of 100 units, but I left it as a Serializable object so that design can
    // mess with the distances if they wanted to.
    [SerializeField] private float _clickDistance = 100f;

    //This is the layermask value of the interactables. All interactables must inherit from the IInteractable Interface and be on the Interactables LayerMask.
    [SerializeField] private LayerMask _clickableLayerMask = 0;
    [SerializeField] private LayerMask _nonImportantClickableLayerMask = 0;

    //This is just a placeholder variable that I use to switch from OnHoverEnter to OnHoverExit
    private IInteractable _previousInteractable;

    private void Update() {
        CheckHover();
        CheckClick();
    }

    private void CheckHover() {
        RaycastHit hit;

        //This basically defines a ray's startpoint that starts from the mouse's position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //returns true if the object it hits is on the Interactables LayerMask and is within 100 units.
        if (Physics.Raycast(ray, out hit, _clickDistance, _clickableLayerMask)) {
            //This finds the object that the ray hits and stores it in an IInteractable variable (similar to _previousInteractable)
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();

            //This is where the placeholder variable comes in. This is how I check to see if the mouse truly moved on or not.
            if (interactable != _previousInteractable) {
                // If the mouse did move on from one object to another, the new object will call OnHoverEnter, and the last object will call OnHoverExit.
                 // Usually, the else statement below will take care of this, since there will be a period of time where the mouse hovers over no interactable
                 // object as it travels from the _previousInteractable to the current interactable, but this is more of a safety measure than anything.
                if (interactable != null) interactable.OnHoverEnter();
                if (_previousInteractable != null) _previousInteractable.OnHoverExit();
                _previousInteractable = interactable;
            }
        }

        //returns true if the raycast hits something that isn't on the Interactables Layer or isn't within 100 units.
        else {
            if (_previousInteractable != null) _previousInteractable.OnHoverExit();
            _previousInteractable = null;
        }
    }

    private void CheckClick() {
        // This is exactly similar to before, but this raycast only gets out if you click the left or right mouse button. I wanted to use another raycast
        // since it allows for separation between hover and click. That way if I somehow mess up hover, click should still work fine. Plus I figured 2
        // raycasts wouldn't cause anyone's pc to crash.
        if (Input.GetMouseButtonDown(0)) {
            Ray rayClick = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayClick, out RaycastHit hitClick, _clickDistance, _clickableLayerMask)) {
                IInteractable interactable = hitClick.transform.GetComponent<IInteractable>();
                interactable?.OnLeftClick();
            }
            else if (Physics.Raycast(rayClick, out hitClick, _clickDistance, _nonImportantClickableLayerMask)) {
                IInteractable interactable = hitClick.transform.GetComponent<IInteractable>();
                interactable?.OnLeftClick(hitClick.point);
            }
        }
        else if (Input.GetMouseButtonDown(1)) {
            Ray rayClick = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayClick, out RaycastHit hitClick, _clickDistance, _clickableLayerMask)) {
                IInteractable interactable = hitClick.transform.GetComponent<IInteractable>();
                interactable?.OnRightClick();
            }
            else if (Physics.Raycast(rayClick, out hitClick, _clickDistance, _nonImportantClickableLayerMask)) {
                IInteractable interactable = hitClick.transform.GetComponent<IInteractable>();
                interactable?.OnRightClick(hitClick.point);
            }
        }
    }
}
*/

