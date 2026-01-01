using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoundData
{
    public List<EnemyWeights> enemyWeights = new List<EnemyWeights>();
    public int roundBudget = 10;
    public int spawnInterval = 60; // seconds
    public EnemyWeights GetEnemyWeights(GameObject enemyPrefab)
    {
        return enemyWeights.Find(e => e.enemyPrefab == enemyPrefab);
    }
}

[Serializable]
public class EnemyWeights
{
    public GameObject enemyPrefab;
    public int weight; //weighted probability
    public int enemyValue; // takes up this amount of the round budget
}