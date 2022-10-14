using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // FIELDS

    public int weightLeft;
    public float avgTimeToSpawn;
    public float avgTimeDeviation;
    public EnemySpawn[] spawnChoices;
    public bool isOpen;

    private float lastSpawnTime;
    private float nextSpawnTime;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        lastSpawnTime = Time.time;
        nextSpawnTime = avgTimeToSpawn + Random.Range(-avgTimeDeviation, avgTimeDeviation);

    }

    // Update is called once per frame
    void Update()
    {
        if(isOpen)
        {
            TryToSpawn();
        }
        
    }

    // METHODS

    private void TryToSpawn()
    {
        if(Time.time > nextSpawnTime && weightLeft > 0)
        {
            // make spawn choice
            int totalWeight = 0;
            foreach(EnemySpawn enemySpawn in spawnChoices)
            {
                totalWeight += enemySpawn.weight;
            }

            int spawnChoice = Random.Range(0, totalWeight);
            EnemySpawn enemySpawnChoice = new EnemySpawn(null, 0); // placeholder enemy spawn

            int spawnWeightSum = 0; // used to track the spawn choice weights before it
            for(int i = 0; i < spawnChoices.Length; i++)
            {
                if(spawnChoice < spawnWeightSum + spawnChoices[i].weight)
                {
                    enemySpawnChoice = spawnChoices[i]; // is this the one?
                    break; // break out of the loop
                }
                else
                {
                    spawnWeightSum += spawnChoices[i].weight; // add the spawn weight before it
                }
                
            }

            // spawn the choice

            Instantiate(enemySpawnChoice.enemy, this.transform.position, this.transform.rotation);
            weightLeft -= enemySpawnChoice.weight;

            // reset spawn timer

            ResetSpawnTimer();
            
        }
    }

    public void ResetSpawnTimer()
    {
        lastSpawnTime = Time.time;
        nextSpawnTime = Time.time + avgTimeToSpawn + Random.Range(-avgTimeDeviation, avgTimeDeviation);
    }

}

[System.Serializable]
// helper struct to wrap an enemy and their spawn weight together
public struct EnemySpawn
{
    // FIELDS

    public Enemy enemy;
    public int weight;

    // CONSTRUCTOR

    public EnemySpawn(Enemy enemy, int weight)
    {
        this.enemy = enemy;
        this.weight = weight;
    }
}
