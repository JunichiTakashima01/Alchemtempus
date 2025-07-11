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
    public GameObject player;

    public TMP_Text enemyRemainingText;
    public TMP_Text coinCntText;
    public int enemyMaxCount = 42;
    private int enemyRemaining;

    private int coinNum = 0;
    public int UpgradeBulletDamageCoinCost = 1;

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

    private void SetCoinNum(int coinCollected)
    {
        coinNum += coinCollected;
        UpdateCoinNum();
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

    public void UpdateBulletDamage()
    {
        if (UseCoin(UpgradeBulletDamageCoinCost))
        {
            player.GetComponent<PlayerShoot>().IncreasePlayerBulletDamageOne();
        }
        UpdateCoinNum();
    }

    public bool UseCoin(int coinNumToUse)
    {
        if (coinNum < coinNumToUse)
        {
            return false;
        }
        else
        {
            coinNum -= coinNumToUse;
            return true;
        }
    }

    private void UpdateCoinNum()
    {
        coinCntText.text = "" + coinNum;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

}
