using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {

    public enum SpawnState
    {
        SPAWNING,
        WAITING,
        COUNTING,
    };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform[] enemyTypes;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;
    public float waveCooldown = 5.0f;

    private float waveCountDown;
    private float minPlayerDistance = 5.0f;

    public Transform[] spawnPoints;

    private SpawnState state = SpawnState.COUNTING;
    private float enemyFindCountdown = 1.0f;

    private Rigidbody2D playerRb2d;
    private GameManager gm;

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Cannot find spawn points");
        }
        if (waves.Length == 0)
        {
            Debug.LogError("Cannot find waves");
        }
         waveCountDown = waveCooldown;

        playerRb2d = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        GameObject gmGo = GameObject.Find("_GM");
        gm = (GameManager)gmGo.GetComponent<GameManager>();
    }

    private void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (!HasEnemyAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (state == SpawnState.COUNTING)
        {
            if (waveCountDown <= 0.0f)
            {
                if (state != SpawnState.SPAWNING)
                {
                    waveCountDown = waveCooldown;
                    StartCoroutine(SpawnWave(waves[nextWave]));
                }
            }
            else
            {
                waveCountDown -= Time.deltaTime;
            }
        }
    }

    private bool HasEnemyAlive()
    {
        enemyFindCountdown -= Time.deltaTime;
        if (enemyFindCountdown <= 0)
        {
            enemyFindCountdown = 1.0f;
            // check only once per second because FindGameObjectWithTag is too taxing
            if (!GameObject.FindGameObjectWithTag("Enemy"))
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator SpawnWave(Wave wave)
    {
        state = SpawnState.SPAWNING;

        // spawn
        for(int i = 0; i< wave.count; i++)
        {
            SpawnEnemy(waves[nextWave].enemyTypes[Random.Range(0, waves[nextWave].enemyTypes.Length)]);
            yield return new WaitForSeconds(1.0f / wave.rate);
        }

        state = SpawnState.WAITING;
        yield break; //returns nothing
    }

    void SpawnEnemy(Transform enemy)
    {
        List<Transform> validSpawnPoints = new List<Transform>();
        foreach (Transform sp in spawnPoints)
        {
            Vector2 spawnLocation2D = new Vector2(sp.position.x, sp.position.y);
            if ((spawnLocation2D - playerRb2d.position).magnitude > minPlayerDistance)
                validSpawnPoints.Add(sp);
        }

        if (validSpawnPoints.Count > 0)
        {
            Transform spawnPoint = validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave completed");
        state = SpawnState.COUNTING;
        nextWave++;
        if (nextWave >= waves.Length)
        {
            // game complete? currently loops
            nextWave = 0;
        }
        gm.StartWave(nextWave + 1);
    }

}
