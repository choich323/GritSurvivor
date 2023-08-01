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
    public Transform uiJoy;

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime;
    public bool isLive;

    [Header("# Player Info")]
    public int playerId;
    public int level;
    public int kill;
    public int exp;
    public ExpManager expManager;
    public float health;
    public float maxHealth;

    void Awake()
    {
        instance = this;
        // 타겟 최대 프레임 설정(PC에서는 더 높은 프레임이 나올 수 있으나, 모바일에서는 60이 최대)
        Application.targetFrameRate = 60;
        expManager = GetComponent<ExpManager>();
        Stop();
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2);
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
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

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
        AudioManager.instance.PlayBgm(false);
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

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
        AudioManager.instance.PlayBgm(false);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        SceneManager.LoadScene("MainMenu");
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
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        // timescale이 증가하면 그만큼 진행속도가 빨라진다
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one;
    }
}
