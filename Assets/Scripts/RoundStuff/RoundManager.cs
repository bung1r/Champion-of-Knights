using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    private PlayerStatManager player;
    private StatsUIManager statsUIManager;
    private SkilltreeManager skilltreeManager;
    private RoundManagerUI roundManagerUI;
    [SerializeField] private ObjectiveDatabase objectiveDatabase;
    [SerializeField] private RoundDatabase roundDatabase;
    [SerializeField] private ViewerItemDatabase viewerItemDatabase;
    [SerializeField] private GameObject audiencePackagePrefab;
    [SerializeField] private GameObject orbSpawnParent;
    [SerializeField] private GameObject orbPrefab;
    private List<Transform> orbSpawns = new List<Transform>(); // automatcially created
    public int currentRound = 0;
    public float roundDuration = 3f;
    public float shopDuration = 3f;
    public float beforeRoundDuration = 3f;
    public float afterRoundDuration = 3f; // time between round end and transition.
    public float updateStatInterval = 0.1f;
    // private float startingViewers = 100f; // start with 100 viewers in order not to rig it. 
    private List<GameObject> currentEnemies = new List<GameObject>();
    private List<Orb> currentOrbs = new List<Orb>();
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
    private int multiKillCount = 0;
    private float lastReceivedGift = -10f;
    private float lastGiftCheck = -10f;
    private float giftCooldown = 10f;
    private float maxGiftTime = 20f;
    private float giftVar = 10000f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;

        player = FindObjectOfType<PlayerStatManager>();
        if (orbSpawnParent != null)
        {
            foreach (Transform child in orbSpawnParent.transform)
            {
                orbSpawns.Add(child);
            }
        }
    }
    void Start()
    {

        StartBeforeRoundIntermission();
    }

    void Update()
    { 

        if (currentRoundState == RoundStates.Active)
        {
            if (player == null) return; 
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
                        obj.currentAmount = multiKillCount;
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

            // audience gift handling
            if (Time.time - lastReceivedGift > giftCooldown && Time.time - lastGiftCheck > 1f && player.stats.viewers > 0f)
            {
                lastGiftCheck = Time.time;
                float giftChance = UnityEngine.Random.Range(0, giftVar);
                giftVar -= 450f;
                if (giftChance < player.stats.viewers || Time.time - lastReceivedGift > maxGiftTime + giftCooldown)
                {
                    giftVar = 10000f;
                    lastReceivedGift = Time.time;
                    lastGiftCheck = Time.time;
                    AudienceGiftEvent();
                }
            }

        } else if (currentRoundState == RoundStates.Shop)
        {
            roundTimer += Time.deltaTime;
            timeRemaining = shopDuration - roundTimer;
            roundManagerUI.UpdateTimer(timeRemaining);
            if (roundTimer >= shopDuration)
            {
                EndShopSequence();
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
    public void AudienceGiftEvent() {
        // give the player a random item as a gift from the audience.
        Item audienceItem = viewerItemDatabase.GetAudienceItem(player.stats.viewers);
        GameObject audiencePackage = Instantiate(audiencePackagePrefab, player.transform.position + Vector3.up * 3f, Quaternion.identity);
        audiencePackage.GetComponent<Package>().item = audienceItem;
    }
    public void StartBeforeRoundIntermission()
    {
        BlackScreen.Instance.FadeFromBlack(1f);
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
        CreateNewObjectives(2,5);
    }
    public void CreateNewObjectives(int min, int max)
    {
        Dictionary<ObjectiveTypes, bool> chosenObjectives = new Dictionary<ObjectiveTypes, bool>();
        for (int i = 0; i < UnityEngine.Random.Range(min, max); i++)
        {
            while (true) {
                ObjectiveScaling objScale = objectiveDatabase.objectiveScalings[UnityEngine.Random.Range(0, objectiveDatabase.objectiveScalings.Count)];
                if (!chosenObjectives.ContainsKey(objScale.objectiveType))
                {
                    chosenObjectives.Add(objScale.objectiveType, true);
                    Objective newObj = objScale.CalculateObjective();
                    currentObjectives.Add(newObj);
                    roundManagerUI.AddEntry(objScale, newObj);
                    if (objScale.objectiveType == ObjectiveTypes.Collect)
                    {
                        SpawnOrbs(newObj.targetAmount);
                    }
                    break;
                }
            }
        }
    }
    private void SetUpRound()
    {
        player.stats.viewers = 0;
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
        multiKillCount = 0;
        sumOfAllViewersThisRound = 0f;
        frameCount = 0f;
        highestViewersThisRound = 0f;
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


        BlackScreen.Instance.FadeToBlackWithDelay(afterRoundDuration - 2f, 2f);
        skilltreeManager.EnableAfterDelay(afterRoundDuration);
        statsUIManager.DisableAfterDelay(afterRoundDuration);
        Debug.Log("Round " + currentRound + " ended, begin shop phase soon.");
    }
    public void StartShopSequence()
    {
        BlackScreen.Instance.FadeFromBlack(2f);


        roundTimer = 0f;
        currentRoundState = RoundStates.Shop;
        Debug.Log("The shop is now open for " + shopDuration + " seconds.");

        

        BlackScreen.Instance.FadeToBlackWithDelay(shopDuration - 2f, 2f);
        skilltreeManager.DisableAfterDelay(shopDuration);
        statsUIManager.EnableAfterDelay(shopDuration);
    }
    public void EndShopSequence()
    {
        Debug.Log("The shop has closed.");
        StartBeforeRoundIntermission();
    }
    public void AssignStatUIManager(StatsUIManager other)
    {
        statsUIManager = other;
    }
    public void AssignSkillTreeManager(SkilltreeManager other)
    {
        skilltreeManager = other;
    }
    public void AssignRoundManagerUIManager(RoundManagerUI other)
    {
        roundManagerUI = other;
    }
    public void SpawnItemAtPos(Item item, Vector3 pos)
    {
        GameObject itemObj = Instantiate(item.prefab, pos, Quaternion.identity);
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
    public void OnMultiKill(int amt) {
        if (amt > multiKillCount) multiKillCount = amt;
    }
    public void CollectOrb()
    {
        orbsCollected++;
    }
    public void SpawnOrbs(int amt)
    {
        if (orbSpawnParent == null) return;
        
        Dictionary<Transform, bool> chosenSpawns = new Dictionary<Transform, bool>();
        for (int i = 0; i < amt; i++)
        {
            while (true)
            {
                Transform chosenSpawn = orbSpawns[UnityEngine.Random.Range(0, orbSpawns.Count)];
                if (chosenSpawns.ContainsKey(chosenSpawn)) continue;
                Orb newOrb = Instantiate(orbPrefab, chosenSpawn.position, Quaternion.identity).GetComponent<Orb>();
                currentOrbs.Add(newOrb);
                chosenSpawns.Add(chosenSpawn, true);
                break;
            }

        }
    }
}

public enum RoundStates
{
    Active, Shop, Intermission, Begin, End, Nothing
}