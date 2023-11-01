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

    float timer;

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
        for (uint i = 0; i <= spawnsPerBurst; i++)
        {
            GameObject newEnemy = Instantiate(enemy, transform.position, Quaternion.identity);
            Vector2 spawnOffset = new Vector3(spawnRadius * Random.Range(0f, 1f), 0, 0);
            spawnOffset = Random.rotation * spawnOffset;
            newEnemy.transform.position = new Vector3(transform.position.x + spawnOffset.x, transform.position.y + spawnOffset.y, 0);
            newEnemy.transform.parent = transform;
        }
    }


}
