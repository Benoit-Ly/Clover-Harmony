using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArenaData", menuName = "Arena/ArenaData", order = 1)]
public class ArenaData : ScriptableObject
{
    public List<WaveData> Waves = new List<WaveData>();

    public void SpawnWave(int waveIndex, Vector3 center, TriggerAbstract trigger)
    {
        Waves[waveIndex].Spawn(center, trigger);
    }
}
