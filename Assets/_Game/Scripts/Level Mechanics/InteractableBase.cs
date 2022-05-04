using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    public virtual void OnHoverEnter() {
    }

    public virtual void OnHoverExit() {
    }

    public virtual void OnLeftClick(Vector3 mousePosition) {
    }
}