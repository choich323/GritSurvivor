using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    public float levelTime;

    int level = 0;
    float timer = 0;

    void Awake()
    {
        // 자식 오브젝트를 통해 초기화
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Start()
    {
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

        if (timer > spawnData[level].spawnTime){
            Spawn();
            timer = 0;
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

// 직렬화를 통해 에디터에 표시되도록 한다.
[System.Serializable]
public class SpawnData
{
    // 타입, 소환 시간, 체력, 속도
    public int spriteType;
    public int health;
    public float spawnTime;
    public float speed;
}