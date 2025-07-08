//using UnityEditor.SearchService;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject gameOverScene;
    public GameObject pauseGameScene;
    public GameObject pauseButtonGameObject;
    public GameObject winGameScene;

    public TMP_Text enemyRemainingText;
    public TMP_Text coinCntText;
    public int enemyMaxCount = 42;
    private int enemyRemaining;

    public int coin_num = 0;

    public static event Action<bool> OnGamePausedChangePauseStatus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverScene.SetActive(false);
        pauseGameScene.SetActive(false);
        winGameScene.SetActive(false);

        PlayerHealth.OnPlayerZeroHealth += GameOverScreen;

        Enemy.OnEnemyKilled += OnEnemyKilled;
        enemyRemaining = enemyMaxCount;
        SetEnemyRemainingText();

        CoinGem.OnCoinGemCollected += SetCoinNum;
    }

    void OnDestroy()
    {
        PlayerHealth.OnPlayerZeroHealth -= GameOverScreen;
        Enemy.OnEnemyKilled -= OnEnemyKilled;
        CoinGem.OnCoinGemCollected -= SetCoinNum;
    }

    private void SetCoinNum(int coin_collected)
    {
        coin_num += coin_collected;
        coinCntText.text = "" + coin_num;
        //Debug.Log(coin_num);
    }

    private void OnEnemyKilled()
    {
        enemyRemaining -= 1;
        if (enemyRemaining <= 0)
        {
            enemyRemaining = 0;
            WinGame();
        }
        SetEnemyRemainingText();
    }

    private void SetEnemyRemainingText()
    {
        enemyRemainingText.text = "Enemy Remaining: " + enemyRemaining + " / " + enemyMaxCount;
    }

    public void WinGame()
    {
        winGameScene.SetActive(true);
    }

    private void GameOverScreen()
    {
        gameOverScene.SetActive(true);
        pauseButtonGameObject.SetActive(false);
        OnGamePausedChangePauseStatus.Invoke(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        OnGamePausedChangePauseStatus.Invoke(false);
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        pauseGameScene.SetActive(true);
        OnGamePausedChangePauseStatus.Invoke(true);
        pauseButtonGameObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pauseGameScene.SetActive(false);
        OnGamePausedChangePauseStatus.Invoke(false);
        pauseButtonGameObject.SetActive(true);
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
