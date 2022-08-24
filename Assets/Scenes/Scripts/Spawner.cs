using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //Array of Wave class.
    public Wave[] waves;
    //Enemy class assigned in inspector
    public Enemy enemy;

    //The current wave
    Wave currentWave;
    //The current waves number
    int currentWaveNumber;

    //The amount of enemies yet to be spawned
    int enemiesRemainingToSpawn;
    //The enemies that are still alive
    int enemiesRemainingAlive;
    //Amount of time before the next enemy is spawned
    float nextSpawnTime;

    private void Start()
    {
        //Start the first wave upon starting the game
        NextWave();
    }

    private void Update()
    {
        //If there are still enemies to spawn, and the adequate time since last spawn has passed, then execute
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            //Set the next time to spawn
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            //Instantiate a new enemy with no rotation, and at the origin. Instantiate as part of the enemy class
            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            //Add a listener to the enemies OnDeath method, to execute OnEnemyDeath function when enemy dies
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    //Executed when enemy dies via listener and delegates
    void OnEnemyDeath()
    {
        //one less enemy remaining alive
        enemiesRemainingAlive--;
        //if no enemies are left alive, start the next wave
        if (enemiesRemainingAlive == 0 )
        {
            NextWave();
        }
    }

    void NextWave()
    {
        //Set the wave number one higher
        currentWaveNumber++;
        //if the current wave index is less than the length of the array, start the next wave
        if (currentWaveNumber - 1 < waves.Length)
        {
            //set the current wave to the next index in the waves array
            currentWave = waves[currentWaveNumber - 1];

            //set the enemies remaining to spawn to the value set for this wave in the navigator
            enemiesRemainingToSpawn = currentWave.enemyCount;
            //Set the enemies that are remaining alive to the value in the navigator
            enemiesRemainingAlive = enemiesRemainingToSpawn;
        }

        
    }

    //Serialize this class in the navigator
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
