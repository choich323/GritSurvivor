using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime;

    [Header("# Player Info")]
    public int level;
    public int kill;
    public int exp;
    public ExpManager expManager;
    public int health;
    public int maxHealth;

    void Awake()
    {
        instance = this;
        expManager = GetComponent<ExpManager>();
    }

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        gameTime += Time.deltaTime;

        if(gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
    }

    public void GetExp()
    {
        exp++;
        if(exp == expManager.nextExp[level - 1])
        {
            level++;
            exp = 0;

        }
    }
}
