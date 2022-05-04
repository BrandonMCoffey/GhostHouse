using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CreditsManager : MonoBehaviour
{
    [SerializeField] private float _speed = 2.5f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.speed = _speed;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _animator.speed = 1f;
        }
        if (!Input.GetKeyDown(KeyCode.Escape) && _animator.GetBool("Done")) return;

        DataManager.SceneLoader.LoadScene("MainMenu");
    }
}