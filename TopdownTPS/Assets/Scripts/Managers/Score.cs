using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static int score { get; private set; }
    public int scorePerEnemy;
    float lastEnemyKilled;
    public static int streakCount;
    public float streakExpireTime = 1;
    // Start is called before the first frame update
    void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerKilled;
    }

    void OnEnemyKilled()
    {
        if (Time.time < lastEnemyKilled + streakExpireTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }
        lastEnemyKilled = Time.time;
        score += scorePerEnemy + (int)Mathf.Pow(2, streakCount);
    }

    void OnPlayerKilled()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;

    }
}
