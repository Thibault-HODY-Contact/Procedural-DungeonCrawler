using UnityEngine;

public class CursorLock : MonoBehaviour
{
    void Update()
    {
        bool isPaused = Time.timeScale == 0f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}