using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public Button switchSceneButton;
    public string nextSceneName;

    public CustomSceneManager sceneManager;

    void Start()
    {
        
    }

    public void SwitchScene()
    {
        //SceneManager.LoadScene("NextSceneName");
        sceneManager.LoadScene(nextSceneName);
    }

    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running as a built application, quit the application
        Application.Quit();
#endif
    }
}
