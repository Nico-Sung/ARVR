using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Settings")]
    public float fallThreshold = -5f;
    public Vector3 spawnPosition = new Vector3(0f, 1.6f, -7f);

    void Update()
    {
        if (transform.position.y < fallThreshold)
            Respawn();
    }

    void Respawn()
    {
        transform.position = spawnPosition;

        // Reset velocity si Rigidbody présent (XR rig physique)
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        // Compatible CharacterController
        var cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            transform.position = spawnPosition;
            cc.enabled = true;
        }
    }
}
