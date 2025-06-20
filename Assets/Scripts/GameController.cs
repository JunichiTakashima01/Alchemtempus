using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject gameOverScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverScene.SetActive(false);
        PlayerHealth.OnPlayerZeroHealth += GameOverScreen;
    }

    private void GameOverScreen()
    {
        gameOverScene.SetActive(true);
        Time.timeScale = 0;
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
