using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CantinaManager : MonoBehaviour
{
    [SerializeField]
    private InputActionReference resetScene;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        resetScene.action.Enable();
        resetScene.action.performed += ResetScene;
    }

    private void ResetScene(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("Cantina");
    }

    private void OnDestroy()
    {
        resetScene.action.Disable();
        resetScene.action.performed -= ResetScene;
    }
}
