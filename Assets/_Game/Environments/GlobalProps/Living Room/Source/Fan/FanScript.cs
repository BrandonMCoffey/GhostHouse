using UnityEngine;

public class FanScript : MonoBehaviour
{
    [SerializeField] public bool _powered;

    private void Start()
    {
        if (_powered) GetComponent<Animator>().SetTrigger("power");
    }

    public void StartFan()
    {
        GetComponent<Animator>().SetTrigger("power");
        _powered = true;
    }
}
