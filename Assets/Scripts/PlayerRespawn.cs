using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Settings")]
    public float fallThreshold = -5f;
    public Vector3 spawnPosition = new Vector3(0f, 0f, -7.5f);

    void Update()
    {
        if (transform.position.y < fallThreshold)
            Respawn();
    }

    void Respawn()
    {
        var cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        transform.position = spawnPosition;

        if (cc != null)
            cc.enabled = true;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }
}
