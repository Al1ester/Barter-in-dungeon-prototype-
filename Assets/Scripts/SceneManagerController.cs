using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneManagerController;

public class SceneManagerController : MonoBehaviour
{
   
    public enum SceneName
    {
        MainMenu,
        Level,
        Death
    }

    private void Start()
    {
        
        DontDestroyOnLoad(gameObject);
    }

    
    public void LoadScene(SceneName sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }

   
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Level");
    }

    public void LoadDeath()
    {
        SceneManager.LoadScene("Death");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

