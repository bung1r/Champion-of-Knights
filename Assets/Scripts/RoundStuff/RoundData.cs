using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoundData
{
    public List<EnemyWeights> enemyWeights = new List<EnemyWeights>();
    public int roundBudget = 10;
    public int spawnInterval = 60; // seconds
    public int minObjectives = 2;
    public int maxObjectives = 2;
    public float enemyScaling = 1; // how much to scale enemy health, damage, and exp bys
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