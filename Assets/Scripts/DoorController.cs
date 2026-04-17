using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle   = 90f;
    [SerializeField] private float rotateSpeed = 2.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [Header("VR Input")]
    [SerializeField] private InputAction triggerAction = new InputAction(
        name: "OpenDoor",
        type: InputActionType.Button,
        binding: "<XRController>{LeftHand}/secondaryButton"
    );

    private Quaternion _closedRot;
    private Quaternion _openRot;
    private bool _isOpen;
    private AudioSource _audioSource;

    void Start()
    {
        _closedRot = transform.localRotation;
        _openRot   = Quaternion.Euler(0, openAngle, 0) * _closedRot;

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        _audioSource.spatialBlend = 1f;
        _audioSource.playOnAwake = false;

        triggerAction.performed += OnTriggerPerformed;
    }

    void Update()
    {
        Quaternion target = _isOpen ? _openRot : _closedRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, rotateSpeed * Time.deltaTime);
    }

    public void ToggleDoor()
    {
        _isOpen = !_isOpen;

        AudioClip clip = _isOpen ? openSound : closeSound;
        if (clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        Debug.Log("Door toggled: " + (_isOpen ? "Open" : "Closed"));
    }

    private void OnTriggerPerformed(InputAction.CallbackContext ctx) => ToggleDoor();

    void OnEnable()
    {
        triggerAction.Enable();
    }

    void OnDisable()
    {
        triggerAction.performed -= OnTriggerPerformed;
        triggerAction.Disable();
    }
}
