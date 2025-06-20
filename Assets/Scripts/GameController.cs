//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject gameOverScene;
    public GameObject pauseGameScene;
    public GameObject pauseButtonGameObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverScene.SetActive(false);
        pauseGameScene.SetActive(false);
        PlayerHealth.OnPlayerZeroHealth += GameOverScreen;
    }

    private void GameOverScreen()
    {
        gameOverScene.SetActive(true);
        Time.timeScale = 0;
    }

    public void PauseGame()
    {
        pauseGameScene.SetActive(true);
        pauseButtonGameObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pauseGameScene.SetActive(false);
        pauseButtonGameObject.SetActive(true);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

}
