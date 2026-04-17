using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle   = 90f;
    [SerializeField] private float rotateSpeed = 2.5f;

    [Header("VR Input")]
    // Binding pré-configuré sur la gâchette droite — modifiable dans l'Inspector
    [SerializeField] private InputAction triggerAction = new InputAction(
        name: "OpenDoor",
        type: InputActionType.Button,
        binding: "<XRController>{LeftHand}/secondaryButton"
    );

    private Quaternion _closedRot;
    private Quaternion _openRot;
    private bool _isOpen = false;

    void Start()
    {
        _closedRot = transform.localRotation;
        _openRot   = Quaternion.Euler(0, openAngle, 0) * _closedRot;

        triggerAction.performed += _ => ToggleDoor();
    }

    void Update()
    {
        Quaternion target = _isOpen ? _openRot : _closedRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, rotateSpeed * Time.deltaTime);
    }

    public void ToggleDoor()
    {
        _isOpen = !_isOpen;
        Debug.Log("Door toggled: " + (_isOpen ? "Open" : "Closed"));
    }

    void OnEnable()
    {
        triggerAction.Enable();
    }

    void OnDisable()
    {
        triggerAction.performed -= _ => ToggleDoor();
        triggerAction.Disable();
    }
}
