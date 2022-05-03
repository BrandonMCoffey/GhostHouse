using UnityEngine;
using UnityEngine.Events;

public class EventOnStart : MonoBehaviour
{
    [SerializeField] private UnityEvent _event = new UnityEvent();

    private void Start() {
        _event.Invoke();
    }
}