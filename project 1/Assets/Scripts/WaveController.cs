using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    // FIELDS

    private Spawner[] spawners;
    public int wave;
    public float difficultySlope;
    public int initialDifficulty;
    public float waveBreakTime; // how long in between waves
    private float lastWaveEnd; // time the last wave ended
    private bool onWaveBreak = false;
    public AudioClip waveStartSound;


    // MONO

    // Start is called before the first frame update
    void Start()
    {
        spawners = FindObjectsOfType<Spawner>();
        GiveSpawnersWeight(GetWeight(wave));

        this.GetComponent<AudioSource>().clip = waveStartSound;
        this.GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
        CheckWave();
    }

    // METHODS

    private void CheckWave()
    {
        if(onWaveBreak)
        {
            // take time 
            if(Time.time - lastWaveEnd > waveBreakTime)
            {
                // initiate next wave
                onWaveBreak = false;
                GiveSpawnersWeight(GetWeight(wave));

                foreach(Spawner spawner in spawners)
                {
                    spawner.ResetSpawnTimer();
                }

                Debug.Log("next wave start");
            }
        }
        else
        {
            // if there are enemies left
            foreach(Enemy enemy in FindObjectsOfType<Enemy>())
            {
                if(enemy.health > 0)
                    return;
            }

            // if the spawner is still going
            foreach(Spawner spawner in spawners)
            {
                if(spawner.weightLeft > 0)
                {
                    return; // not ready for next wave yet
                }
            }
            // then keep the wave going
            // else... take break for next wave

            Debug.Log("next wave break");
            wave++;
            lastWaveEnd = Time.time;
            onWaveBreak = true;

            this.GetComponent<AudioSource>().clip = waveStartSound;
            this.GetComponent<AudioSource>().Play();
        }

        
    }

    private void GiveSpawnersWeight(int howMuch)
    {
        foreach(Spawner spawner in spawners)
        {
            spawner.weightLeft = howMuch;
        }
    }

    private int GetWeight(int wave)
    {
        if(wave % 5 == 0)
        {
            return (int)(wave * difficultySlope) + (int)(2 * wave * difficultySlope) + initialDifficulty;
        }
        else
        {
            return (int)(wave * difficultySlope) + initialDifficulty;
        }

    }

}
