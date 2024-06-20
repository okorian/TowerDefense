using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField] EnemyData _defaultEnemy;
    [SerializeField] EnemyData _fastEnemy;
    [SerializeField] EnemyData _flyingEnemy;
    [SerializeField] EnemyData _goldEnemy;
    [SerializeField] EnemyData _scalingEnemy;
    [SerializeField] EnemyData _splittingEnemy;
    [SerializeField] EnemyData _tankyEnemy;

    int _mult = 1;

    public EnemyWave GenerateWave(int round)
    {
        EnemyWave wave = new EnemyWave();
        List<EnemyData> enemys = new List<EnemyData>();

        if(round % 10 == 0)
        {
            _mult++;
        }

        if(round <= 10)
        {
            wave.spawnSpeed = 2.0f;
        }
        else if (round <= 15)
        {
            wave.spawnSpeed = 1.5f;
        }
        else if (round <= 20)
        {
            wave.spawnSpeed = 1.0f;
        }
        else if (round <= 25)
        {
            wave.spawnSpeed = 0.75f;
        }
        else if (round <= 30)
        {
            wave.spawnSpeed = 0.5f;
        }
        else
        {
            wave.spawnSpeed = 0.25f;
        }

        for (int i = 0; i < 4 + round * _mult; i++)
        {
            enemys.Add(_defaultEnemy);
        }

        if(round % 5 == 0)
        {
            for (int i = 0; i < round * _mult -1; i++)
            {
                enemys.Add(_flyingEnemy);
            }
        }

        if (round >= 3)
        {
            for (int i = 0; i < (round / 3) * _mult; i++)
            {
                enemys.Add(_fastEnemy);
            }
        }

        if (round >= 8)
        {
            for (int i = 0; i < ((round / 2) - 3) * _mult-1; i++)
            {
                enemys.Add(_tankyEnemy);
            }
        }

        if (round >= 10)
        {
            for (int i = 0; i < (round - 5) * _mult-1; i++)
            {
                enemys.Add(_splittingEnemy);
            }
        }

        if (round > 15)
        {
            for (int i = 0; i < (round - 15) * _mult-1; i++)
            {
                enemys.Add(_scalingEnemy);
            }
        }

        if (round >= 3 && Random.Range(0, 20) == 0)
        {
            enemys.Add(_goldEnemy);
        }


        wave.enemyQueue = RandomizeWaveOrder(enemys);
        return wave;
    }

    private Queue<EnemyData> RandomizeWaveOrder(List<EnemyData> list)
    {
        int n = list.Count;

        // Fisher-Yates-Shuffle Algorithmus
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            EnemyData data = list[k];
            list[k] = list[n];
            list[n] = data;
        }

        return new Queue<EnemyData>(list);
    }
}
