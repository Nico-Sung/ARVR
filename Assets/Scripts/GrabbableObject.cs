using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class GrabbableObject : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private bool returnToSpawnOnDrop = false;
    [SerializeField] private float returnDelay = 3f;

    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;
    private XRGrabInteractable _grab;
    private Rigidbody _rb;
    private float _dropTime;
    private bool _isDropped;

    private void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        _rb = GetComponent<Rigidbody>();

        _spawnPosition = transform.position;
        _spawnRotation = transform.rotation;

        _grab.selectEntered.AddListener(OnGrabbed);
        _grab.selectExited.AddListener(OnReleased);
    }

    private void OnDestroy()
    {
        _grab.selectEntered.RemoveListener(OnGrabbed);
        _grab.selectExited.RemoveListener(OnReleased);
    }

    private void Update()
    {
        if (returnToSpawnOnDrop && _isDropped && Time.time - _dropTime >= returnDelay)
            ReturnToSpawn();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _isDropped = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (returnToSpawnOnDrop)
        {
            _isDropped = true;
            _dropTime = Time.time;
        }
    }

    public void ReturnToSpawn()
    {
        _isDropped = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(_spawnPosition, _spawnRotation);
    }
}
