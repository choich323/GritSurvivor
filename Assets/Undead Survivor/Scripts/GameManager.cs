using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiGameOver;
    public GameObject enemyCleaner;

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime;
    public bool isLive;

    [Header("# Player Info")]
    public int level;
    public int kill;
    public int exp;
    public ExpManager expManager;
    public float health;
    public float maxHealth;

    void Awake()
    {
        instance = this;
        expManager = GetComponent<ExpManager>();
        Stop();
    }

    public void GameStart()
    {
        health = maxHealth;
        uiLevelUp.Select(0);
        Resume();
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRountine());
    }

    IEnumerator GameOverRountine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiGameOver.gameObject.SetActive(true);
        uiGameOver.Lose();
        Stop();
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRountine());
    }

    IEnumerator GameVictoryRountine()
    {
        isLive = false;

        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiGameOver.gameObject.SetActive(true);
        uiGameOver.Win();
        Stop();
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (!isLive) return;

        gameTime += Time.deltaTime;

        if(gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }
    }

    public void GetExp()
    {
        if (!isLive)
            return;

        exp++;
        if(exp >= expManager.nextExp[Mathf.Min(level - 1, expManager.nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        // timescale이 증가하면 그만큼 진행속도가 빨라진다
        Time.timeScale = 1;
    }
}
