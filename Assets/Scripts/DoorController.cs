using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle   = 90f;
    [SerializeField] private float rotateSpeed = 2.5f;

    [Header("Input")]
    [SerializeField] private InputAction action;

    private Quaternion _closedRot;
    private Quaternion _openRot;
    private bool _isOpen = false;

    void Start()
    {
        _closedRot = transform.localRotation;
        _openRot   = Quaternion.Euler(0, openAngle, 0) * _closedRot;
    }

    void Update()
    {
        if (action.triggered)
        {
            _isOpen = !_isOpen;
            Debug.Log("Door toggled: " + (_isOpen ? "Open" : "Closed"));
        }

        Quaternion target = _isOpen ? _openRot : _closedRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, rotateSpeed * Time.deltaTime);
    }

    public void OnEnable()
    {
        action.Enable();
    }

    public void OnDisable()
    {
        action.Disable();
    }
}
