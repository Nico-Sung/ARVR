using UnityEngine;

/// <summary>
/// Attach to the XR Origin (or any root that moves with the player).
/// Plays footstep clips when horizontal movement is detected.
/// </summary>
public class FootstepSound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Settings")]
    [SerializeField] private float stepInterval = 0.45f;
    [SerializeField] private float minSpeedThreshold = 0.15f;
    [SerializeField, Range(0f, 1f)] private float volume = 0.5f;

    private AudioSource _audioSource;
    private Vector3 _previousPosition;
    private float _stepTimer;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        _audioSource.spatialBlend = 0f;
        _audioSource.playOnAwake = false;

        _previousPosition = transform.position;
    }

    private void Update()
    {
        if (footstepClips == null || footstepClips.Length == 0)
            return;

        Vector3 currentPos = transform.position;
        Vector3 delta = currentPos - _previousPosition;
        delta.y = 0f;

        float horizontalSpeed = delta.magnitude / Time.deltaTime;
        _previousPosition = currentPos;

        if (horizontalSpeed < minSpeedThreshold)
        {
            _stepTimer = 0f;
            return;
        }

        _stepTimer += Time.deltaTime;

        if (_stepTimer >= stepInterval)
        {
            _stepTimer = 0f;
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            _audioSource.PlayOneShot(clip, volume);
        }
    }
}