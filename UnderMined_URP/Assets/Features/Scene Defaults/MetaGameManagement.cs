using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MetaGameManagement : MonoBehaviour
{
    private bool paused = false;
    
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Time.timeScale = paused ? 0 : 1f;
        }
        
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
