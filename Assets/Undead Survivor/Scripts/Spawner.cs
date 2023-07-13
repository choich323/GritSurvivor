using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level = 0;
    float timer = 0;

    void Awake()
    {
        // �ڽ� ������Ʈ�� ���� �ʱ�ȭ
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.Instance.gameTime / 10f), spawnData.Length - 1);

        if (timer > spawnData[level].spawnTime){
            Spawn();
            timer = 0;
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.Instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

// ����ȭ�� ���� �����Ϳ� ǥ�õǵ��� �Ѵ�.
[System.Serializable]
public class SpawnData
{
    // Ÿ��, ��ȯ �ð�, ü��, �ӵ�
    public int spriteType;
    public int health;
    public float spawnTime;
    public float speed;
}