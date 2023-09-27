using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public bool IsgamePaused;

    private void Awake()
    {
        IsgamePaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// when pause the game, mouse will unlock
    /// </summary>
    public void PauseGame()
    {
        if (IsgamePaused == true)
        {
            // stop -> continue
            IsgamePaused = false;
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            print("set to continue.");
        }
        else if (IsgamePaused == false)
        {
            // continue -> stop
            IsgamePaused = true;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            print("set to stop.");
        }
    }
}
