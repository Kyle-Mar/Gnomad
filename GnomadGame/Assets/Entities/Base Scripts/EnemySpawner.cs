using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float timeBetweenBurst = 8;
    [SerializeField] uint spawnsPerBurst = 2;
    [SerializeField] GameObject enemy;
    [SerializeField] float spawnRadius = 5;
    [Tooltip("Wait for entities to die before spawning more?")]
    [SerializeField] bool SpawnContinuous = false;

    float timer;
    private List<GameObject> enemies = new List<GameObject>();

    private void Awake()
    {
        SpawnEnemies();
        timer = timeBetweenBurst;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnEnemies();
            timer = timeBetweenBurst;
        }   
    }
    void SpawnEnemies()
    {
        uint enemiesToSpawn = spawnsPerBurst;

        enemies.RemoveAll(x => !x);

        enemies.TrimExcess();


        if (!SpawnContinuous) { enemiesToSpawn -= (uint)enemies.Count; }

        if (enemiesToSpawn > 100) { Debug.Log("Spawning way too many entities"); return; }
        for (uint i = 0; i < enemiesToSpawn; i++)
        {
            GameObject newEnemy = Instantiate(enemy, transform.position, Quaternion.identity);
            Vector2 spawnOffset = new Vector3(spawnRadius * Random.Range(0f, 1f), 0, 0);
            spawnOffset = Random.rotation * spawnOffset;
            newEnemy.transform.position = new Vector3(transform.position.x + spawnOffset.x, transform.position.y + spawnOffset.y, 0);
            newEnemy.transform.parent = transform;
            enemies.Add(newEnemy);
        }
    }


}
