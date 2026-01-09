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
    [SerializeField] private GameObject packageDronePrefab;
    [SerializeField] private GameObject audiencePackagePrefab;
    [SerializeField] private GameObject orbSpawnParent;
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private AbilityEquipUIManager abilityEquipUIManager;
    [SerializeField] private RoundSummaryManagerUI roundSummaryManagerUI;
    [SerializeField] private GameOverUIManager gameOverCanvas;
    [SerializeField] private GameOverUIManager victoryCanvas;
    [SerializeField] private SimpleEnableText packageDropText;
    [SerializeField] private GameObject PRISON;
    [SerializeField] private GameObject spawnLocationsParent;
    private List<Transform> orbSpawns = new List<Transform>(); // automatcially created
    public int currentRound = 0;
    public float roundDuration = 3f;
    public float shopDuration = 3f;
    public float beforeRoundDuration = 3f;
    public float afterRoundDuration = 3f; // time between round end and transition.
    public float updateStatInterval = 0.1f;
    // private float startingViewers = 100f; // start with 100 viewers in order not to rig it. 
    private List<GameObject> currentEnemies = new List<GameObject>();
    private List<GameObject> currentPackages = new List<GameObject>();
    private List<GameObject> currentOnGroundItems = new List<GameObject>();
    private List<Orb> currentOrbs = new List<Orb>();
    public bool isRoundActive = false;
    public RoundStates currentRoundState = RoundStates.Nothing;
    public float enemyScaling = 1f; // one is normal, increase by 0.2 every round 
    // below are all the main stats tracked for rounds.
    [Space(10)]
    [Header("Fun Stuff!")]
    public bool RIGGED = false;
    public int RIGGEDSPAWN = -1;
    private List<Objective> currentObjectives = new List<Objective>();
    private RoundData currentRoundData;
    private int timesParried = 0;
    private int orbsCollected = 0;
    private int enemiesKilled = 0;
    private int objectivesCompleted = 0;
    // time related things
    private float roundTimer = 0f;
    private float timeRemaining = 0f;
    private float lastUpdatedStat = 0f;
    private float lastSpawnedEnemies = -999f;
    private float sumOfAllViewersThisRound = 0f;
    private float frameCount = 0f;
    private int highestGradeThisRound = 0;
    private float highestViewersThisRound = 0f;
    private int multiKillCount = 0;
    private float lastReceivedGift = -10f;
    private float lastGiftCheck = -10f;
    private float giftCooldown = 10f;
    private float maxGiftTime = 30f;
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
            if (player == null) {
                EndCurrentRound();
                return; 
            }
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
            if ((int)player.stats.styleLevel > highestGradeThisRound)
            {
                highestGradeThisRound = (int)player.stats.styleLevel;
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
                giftVar -= 300f;
                if (giftChance < player.stats.viewers || Time.time - lastReceivedGift > maxGiftTime + giftCooldown)
                {
                    giftVar = 20000f;
                    lastReceivedGift = Time.time;
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
                if (RIGGED) {
                    StartVictorySequence(); 
                    return;
                }
                if (objectivesCompleted == currentObjectives.Count && player != null)
                {
                    if (currentRound >= 10)
                    {
                        //completed all rounds? You win!
                        StartVictorySequence();
                    } else
                    {
                        StartShopSequence();
                    }
                } 
                {
                    // didn't complete all objectives? You lose. 
                    StartGameOverSequence();
                }
            }
        } else if (currentRoundState == RoundStates.GameOver)
        {
            // do nothing for now.
        } else if (currentRoundState == RoundStates.GameVictory)
        {
            // do nothing for now.
        }
    

    }
    public void AudienceGiftEvent() {
        // give the player a random item as a gift from the audience.
        Item audienceItem = viewerItemDatabase.GetAudienceItem(player.stats.viewers);
        Debug.Log("The audience has sent you a gift: " + audienceItem.itemName);
        // create the drone that brings the package 
        GameObject packageDrone = Instantiate(packageDronePrefab, player.transform.position + Vector3.up * 5f + Vector3.forward * 6f, Quaternion.identity);
        DronePackage dronePackage = packageDrone.GetComponent<DronePackage>();
        dronePackage.item = audienceItem;
        dronePackage.packagePrefab = audiencePackagePrefab;
        dronePackage.targetPos = new Vector3(player.transform.position.x, player.transform.position.y + 3f, player.transform.position.z);
        packageDrone.GetComponent<Rigidbody>().velocity = (dronePackage.targetPos - packageDrone.transform.position).normalized * 5f;
    
        // set the text
        packageDropText.EnableTextForSeconds(2f);

        // GameObject audiencePackage = Instantiate(audiencePackagePrefab, player.transform.position + Vector3.up * 3f, Quaternion.identity);
        // audiencePackage.GetComponent<Package>().item = audienceItem;
    }
    public void StartBeforeRoundIntermission()
    {
        BlackScreen.Instance.FadeFromBlack(1f);
        KnightSpawnIn();
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
        CreateNewObjectives(currentRoundData.minObjectives,currentRoundData.maxObjectives);
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
        highestGradeThisRound = 0;
    }
    private void CleanUpRound()
    {
        // clean up any remaining enemies, packages, items, orbs, etc.

        // cleanup all the enemies 
        for (int i = currentEnemies.Count - 1; i >= 0; i--)
        {
            if (currentEnemies[i] != null)
            {
                Destroy(currentEnemies[i]);
            }
            currentEnemies.RemoveAt(i);
        }

        // cleanup all the packages 
        for (int i = currentPackages.Count - 1; i >= 0; i--)
        {
            if (currentPackages[i] != null)
            {
                Destroy(currentPackages[i]);
            }
            currentPackages.RemoveAt(i);
        }

        // cleanup all the on ground items
        for (int i = currentOnGroundItems.Count - 1; i >= 0; i--)
        {
            if (currentOnGroundItems[i] != null)
            {
                Destroy(currentOnGroundItems[i]);
            }
            currentOnGroundItems.RemoveAt(i);
        }

        // clean up all the orbs
        for (int i = currentOrbs.Count - 1; i >= 0; i--)
        {
            if (currentOrbs[i] != null)
            {
                Destroy(currentOrbs[i].gameObject);
            }
            currentOrbs.RemoveAt(i);
        }
    }
    public void EndCurrentRound()
    {
        currentRoundState = RoundStates.End;
        roundTimer = 0f;
        isRoundActive = false;
        
        CleanUpRound();

        foreach (Objective obj in currentObjectives)
        {
            if (obj.IsCompleteFull()) objectivesCompleted++;
        }

        // round summary UI popup and update 
        UpdateRoundSummaryUI();

        // disabling some UI to prepare for next phase
        BlackScreen.Instance.FadeToBlackWithDelay(afterRoundDuration - 2f, 2f);
        statsUIManager.DisableAfterDelay(afterRoundDuration);
        roundSummaryManagerUI.DisableAfterDelay(afterRoundDuration);

        if (objectivesCompleted == currentObjectives.Count)
        {
            // continue if you did all the objectives
            skilltreeManager.EnableAfterDelay(afterRoundDuration);
            Debug.Log("Round " + currentRound + " ended, begin shop phase soon.");
        }



    }
    public void StartShopSequence()
    {
        BlackScreen.Instance.FadeFromBlack(2f);

        // send player to prison so they don't kill themselves or something
        SENDTOPRISON();

        roundTimer = 0f;
        currentRoundState = RoundStates.Shop;
        Debug.Log("The shop is now open for " + shopDuration + " seconds.");

        

        BlackScreen.Instance.FadeToBlackWithDelay(shopDuration - 2f, 2f);
        skilltreeManager.DisableAfterDelay(shopDuration);
        abilityEquipUIManager.DisableAfterDelay(shopDuration);
        statsUIManager.EnableAfterDelay(shopDuration);
    }
    public void EndShopSequence()
    {
        Debug.Log("The shop has closed.");
        StartBeforeRoundIntermission();
    }
    public void StartGameOverSequence()
    {
        BlackScreen.Instance.FadeFromBlack(2f);
        roundManagerUI.DisableTimer();
        currentRoundState = RoundStates.GameOver;
        SENDTOPRISON();

        if (player == null)
        {
            gameOverCanvas.EnableUI("You have died. Now you'll never make it back home.");
        } else if (objectivesCompleted < currentObjectives.Count) 
        {
            gameOverCanvas.EnableUI("You failed to complete all objectives. You will be punished.");
        } else
        {
            gameOverCanvas.EnableUI("Hey :LOL!!! IDK HOW YOU GOT HERE XD you GOT ME!!!! you lost tho...");
        }
    }
    public void StartVictorySequence()
    {
        BlackScreen.Instance.FadeFromBlack(2f);
        roundManagerUI.DisableTimer();
        currentRoundState = RoundStates.GameVictory;
        SENDTOPRISON();
        victoryCanvas.EnableUI("You have completed 10 rounds, and may finally return home! Congratulations!");
    }
    public void UpdateRoundSummaryUI()
    {
        roundSummaryManagerUI.UpdateObjectives(objectivesCompleted, currentObjectives.Count);
        roundSummaryManagerUI.UpdateGrade(((StyleGrades)highestGradeThisRound).ToString());
        roundSummaryManagerUI.UpdateViewerCount((int)highestViewersThisRound);
        roundSummaryManagerUI.UpdateKills(enemiesKilled);
        roundSummaryManagerUI.UpdateParries(timesParried);
        roundSummaryManagerUI.UpdateRoundCount(currentRound);
        roundSummaryManagerUI.EnableAfterDelay(0.2f);
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
    public void AddPackage(GameObject package)
    {
        currentPackages.Add(package);
    }
    public void AddOnGroundItem(GameObject item)
    {
        currentOnGroundItems.Add(item);
    }
    public void SENDTOPRISON()
    {
        if (player == null) return;
        player.transform.position = PRISON.transform.position + Vector3.up * 2f;
    }
    public void KnightSpawnIn()
    {
        if (RIGGEDSPAWN >= 0 && RIGGEDSPAWN < spawnLocationsParent.transform.childCount) {
            player.transform.position = spawnLocationsParent.transform.GetChild(RIGGEDSPAWN).position;
            return; 
        }
        player.transform.position = spawnLocationsParent.transform.GetChild(UnityEngine.Random.Range(0, spawnLocationsParent.transform.childCount)).position;
    }
}

public enum RoundStates
{
    Active, Shop, Intermission, Begin, End, Nothing, GameOver, GameVictory
}