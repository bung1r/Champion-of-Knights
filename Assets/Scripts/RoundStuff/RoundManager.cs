using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    private PlayerStatManager player;
    private StatsUIManager statsUIManager;
    private RoundManagerUI roundManagerUI;
    [SerializeField] private ObjectiveDatabase objectiveDatabase;
    [SerializeField] private RoundDatabase roundDatabase;
    public int currentRound = 0;
    public float roundDuration = 3f;
    public float shopDuration = 3f;
    public float beforeRoundDuration = 3f;
    public float afterRoundDuration = 3f; // time between round end and transition.
    public float updateStatInterval = 0.1f;
    // private float startingViewers = 100f; // start with 100 viewers in order not to rig it. 
    private List<GameObject> currentEnemies = new List<GameObject>();
    
    public bool isRoundActive = false;
    public RoundStates currentRoundState = RoundStates.Nothing;
    // below are all the main stats tracked for rounds.
    private List<Objective> currentObjectives = new List<Objective>();
    private RoundData currentRoundData;
    private int timesParried = 0;
    private int orbsCollected = 0;
    private int enemiesKilled = 0;
    // time related things
    private float roundTimer = 0f;
    private float timeRemaining = 0f;
    private float lastUpdatedStat = 0f;
    private float lastSpawnedEnemies = -999f;
    private float sumOfAllViewersThisRound = 0f;
    private float frameCount = 0f;
    private float highestViewersThisRound = 0f;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;

        player = FindObjectOfType<PlayerStatManager>();
    }
    void Start()
    {
        StartBeforeRoundIntermission();
    }

    void Update()
    {   

        if (currentRoundState == RoundStates.Active)
        {
            roundTimer += Time.deltaTime;
            timeRemaining = roundDuration - roundTimer;
            roundManagerUI.UpdateTimer(timeRemaining);
            
            
            if (roundTimer >= roundDuration)
            {
                EndCurrentRound();
                return;
            }

            frameCount++;
            sumOfAllViewersThisRound += player.stats.viewers;
            float avgViewersThisRound = sumOfAllViewersThisRound / frameCount;
            if (player.stats.viewers > highestViewersThisRound)
            {
                highestViewersThisRound = player.stats.viewers;
            }

            // objective handling.
            if (currentObjectives.Count > 0 && Time.time - lastUpdatedStat > updateStatInterval)
            {
                lastUpdatedStat = Time.time;
                foreach (Objective obj in currentObjectives)
                {
                    if (obj.IsComplete()) continue;
                    if (obj.objectiveType == ObjectiveTypes.Collect)
                    {
                        obj.currentAmount = orbsCollected;
                    } else if (obj.objectiveType == ObjectiveTypes.Kill)
                    {
                        obj.currentAmount = enemiesKilled;
                    } else if (obj.objectiveType == ObjectiveTypes.Parry)
                    {
                        obj.currentAmount = timesParried;
                    } else if (obj.objectiveType == ObjectiveTypes.Viewers)
                    {
                        obj.currentAmount = (int)avgViewersThisRound;
                    } else if (obj.objectiveType == ObjectiveTypes.ViewersOneTime)
                    {
                        obj.currentAmount = (int)highestViewersThisRound;
                    } else if (obj.objectiveType == ObjectiveTypes.Multikill)
                    {
                        
                    } else if (obj.objectiveType == ObjectiveTypes.StyleLevel)
                    {
                        obj.currentAmount = (int)player.stats.styleLevel;
                    }

                    
                }
            }

            //enemy spawn handling
            if (Time.time - lastSpawnedEnemies > currentRoundData.spawnInterval)
            {
                lastSpawnedEnemies = Time.time;
                int totalValue = 0;
                

                // spawn enemies based on weights until budget is reached.
                List<GameObject> enemyPool = currentRoundData.enemyWeights.Select(e => e.enemyPrefab).ToList();
                List<int> weightPool = currentRoundData.enemyWeights.Select(e => e.weight).ToList();
                List<GameObject> enemiesToSpawn = new List<GameObject>();
                while (totalValue < currentRoundData.roundBudget)
                {
                    GameObject chosenEnemy = WeightedRandom.Choose(enemyPool, weightPool);
                    EnemyWeights enemyWeight = currentRoundData.GetEnemyWeights(chosenEnemy);
                    enemiesToSpawn.Add(chosenEnemy);
                    totalValue += enemyWeight.enemyValue;
                }
                
                SpawnPlatformManager.Instance.SpawnEnemies(enemiesToSpawn, player.transform.position);

            }

        } else if (currentRoundState == RoundStates.Shop)
        {
            roundTimer += Time.deltaTime;
            timeRemaining = shopDuration - roundTimer;
            roundManagerUI.UpdateTimer(timeRemaining);
            if (roundTimer >= shopDuration)
            {
                StartBeforeRoundIntermission();
            }
        } else if (currentRoundState == RoundStates.Begin)
        {
            roundTimer += Time.deltaTime;
            timeRemaining = beforeRoundDuration - roundTimer;
            roundManagerUI.UpdateTimer(timeRemaining);
            if (roundTimer >= beforeRoundDuration)
            {
                StartNewRound();
            }
        } else if (currentRoundState == RoundStates.End)
        {
            roundTimer += Time.deltaTime;
            timeRemaining = afterRoundDuration - roundTimer;
            roundManagerUI.UpdateTimer(timeRemaining);
            if (roundTimer >= afterRoundDuration)
            {
                StartShopSequence();
            }
        }
    

    }
    public void StartBeforeRoundIntermission()
    {
        currentRoundState = RoundStates.Begin;
        roundTimer = 0f;
        Debug.Log("Round will begin soon!");
    }
    public void StartNewRound()
    {
        // basic setup for a new round.
        SetUpRound();

        //assign the objectives for the round. 
        currentObjectives.Clear();
        roundManagerUI.ClearEntries();
        Dictionary<ObjectiveTypes, bool> chosenObjectives = new Dictionary<ObjectiveTypes, bool>();
        for (int i = 0; i < UnityEngine.Random.Range(2, 5); i++)
        {
            while (true) {
                ObjectiveScaling objScale = objectiveDatabase.objectiveScalings[UnityEngine.Random.Range(0, objectiveDatabase.objectiveScalings.Count)];
                if (!chosenObjectives.ContainsKey(objScale.objectiveType))
                {
                    chosenObjectives.Add(objScale.objectiveType, true);
                    Objective newObj = objScale.CalculateObjective();
                    currentObjectives.Add(newObj);
                    roundManagerUI.AddEntry(objScale, newObj);
                    break;
                }
            }
        }
    }
    public void CreateNewObjective()
    {
        
    }
    private void SetUpRound()
    {
        currentRoundState = RoundStates.Active;
        currentRound++;
        currentRoundData = roundDatabase.GetRoundData(currentRound);
        isRoundActive = true;
        roundTimer = 0f;
        lastUpdatedStat = 0f;
        lastSpawnedEnemies = -999f;
        timesParried = 0;
        orbsCollected = 0;
        enemiesKilled = 0;
    }
    public void EndCurrentRound()
    {
        currentRoundState = RoundStates.End;
        roundTimer = 0f;
        isRoundActive = false;

        // cleanup all the enemies 
        for (int i = currentEnemies.Count - 1; i >= 0; i--)
        {
            if (currentEnemies[i] != null)
            {
                Destroy(currentEnemies[i]);
            }
            currentEnemies.RemoveAt(i);
        }
        Debug.Log("Round " + currentRound + " ended, begin shop phase soon.");
    }
    public void StartShopSequence()
    {
        roundTimer = 0f;
        currentRoundState = RoundStates.Shop;


        Debug.Log("The shop is now open for " + shopDuration + " seconds.");
    }
    

    public void AssignStatUIManager(StatsUIManager other)
    {
        statsUIManager = other;
    }
    public void AssignRoundManagerUIManager(RoundManagerUI other)
    {
        roundManagerUI = other;
    }
    public void AddEnemy(GameObject enemy)
    {
        currentEnemies.Add(enemy);
    }
    public void OnParry()
    {
        timesParried++;
    }
    public void OnOrbCollected()
    {
        orbsCollected++;
    }
    public void OnEnemyKilled()
    {
        enemiesKilled++;   
    }
}

public enum RoundStates
{
    Active, Shop, Intermission, Begin, End, Nothing
}