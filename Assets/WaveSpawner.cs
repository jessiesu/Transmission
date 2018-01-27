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
        public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;
    public float waveCooldown = 5.0f;
    private float waveCountdown = 0;

    public Transform[] spawnPoints;

    private SpawnState state = SpawnState.COUNTING;
    private float enemyFindCountdown = 1.0f;

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

        waveCountdown = waveCooldown;
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

        if (waveCooldown <= 0.0f)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));            }
        }
        else
        {
            waveCooldown -= Time.deltaTime;
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
            SpawnEnemy(wave.enemy);
            yield return new WaitForSeconds(1.0f / wave.rate);
        }

        state = SpawnState.WAITING;
        yield break; //returns nothing
    }

    void SpawnEnemy(Transform enemy)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }

    void WaveCompleted()
    {
        Debug.Log("Wave completed");

        state = SpawnState.COUNTING;
        waveCountdown = waveCooldown;
        nextWave++;
        if (nextWave >= waves.Length)
        {
            // game complete? currently loops
            nextWave = 0;
        }
    }

}
