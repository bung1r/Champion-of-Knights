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
    [SerializeField] private ObjectiveDatabase manualObjectiveDatabase; // must trigger manually, such as the boss kill one.
    [SerializeField] private RoundDatabase roundDatabase;
    [SerializeField] private RoundDatabase tutorialRoundDatabase; 
    [SerializeField] private ViewerItemDatabase viewerItemDatabase;
    [SerializeField] private GameObject packageDronePrefab;
    [SerializeField] private GameObject audiencePackagePrefab;
    [SerializeField] private GameObject orbSpawnParent;
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private AbilityEquipUIManager abilityEquipUIManager;
    [SerializeField] private RoundSummaryManagerUI roundSummaryManagerUI;
    [SerializeField] private Canvas openSkillTreeCanvas;
    [SerializeField] private GlitchEffectController glitchEffectController;
    [SerializeField] private GameOverUIManager gameOverCanvas;
    [SerializeField] private GameOverUIManager victoryCanvas;
    [SerializeField] public SimpleEnableText packageDropText;
    [SerializeField] private GameObject PRISON;
    [SerializeField] private GameObject spawnLocationsParent;
    private List<Transform> orbSpawns = new List<Transform>(); // automatcially created
    public int currentRound = 0;
    public float roundDuration = 3f;
    public float finalRoundDuration = 6000f; // 10 minutes. If you somehow run out of time, you're a bum.
    public float shopDuration = 3f;
    private bool blackStartedFadingShop = false;
    public float beforeRoundDuration = 3f;
    public float afterRoundDuration = 3f; // time between round end and transition.
    public float updateStatInterval = 0.1f;
    // private float startingViewers = 100f; // start with 100 viewers in order not to rig it. 
    private List<GameObject> currentEnemies = new List<GameObject>();
    private List<GameObject> currentPackages = new List<GameObject>();
    private List<GameObject> currentOnGroundItems = new List<GameObject>();
    [HideInInspector] public List<DatabaseItemData> databaseItemDatas = new List<DatabaseItemData>();
    private List<Orb> currentOrbs = new List<Orb>();
    public bool isRoundActive = false;
    public RoundStates currentRoundState = RoundStates.Nothing;
    public float enemyScaling = 1f; // one is normal, increase by 0.2 every round 
    public string midGameChoice = "";
    public float introCutsceneLength = 2f;
    public const int finalRound = 7;
    // below are all the main stats tracked for rounds.
    [Space(10)]
    [Header("Fun Stuff!")]
    public bool DEBUGMODE = false;
    public bool RIGGED = false;
    public bool INFINITERIGGED = false;
    public int RIGGEDSPAWN = -1;
    public bool JOURNALISTMODE = false;
    public int STARTING_SKILL_POINTS = 0;
    public bool START_WITH_BEGIN = false;
    public bool START_WITH_NOTHING = false;
    private List<Objective> currentObjectives = new List<Objective>();
    private RoundData currentRoundData;
    private bool didTutorialObjectivesFlag = false;
    public int ending = -1; // 1 = A, 2 = B, 3 = C...
    private int timesParried = 0;
    private int orbsCollected = 0;
    private int enemiesKilled = 0;
    private int objectivesCompleted = 0;
    private int initialLevel = 0;
    private int afterRoundLevel = 0;
    private int bonusPointGain = 0;

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
    private float giftCooldown = 15f;
    private float maxGiftTime = 30f;
    private float giftVar = 10000f;
    private int loyalViewersGained = 0;
    private int repGained = 0;
    private int corruptionGained = 0;
    
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

        if (DEBUGMODE == false)
        {
            player.transform.position = new Vector3(0f, 0.5f, 300f);
            STARTING_SKILL_POINTS = 0;
            START_WITH_BEGIN = false;
            START_WITH_NOTHING = false;
            roundDuration = 180f;
            finalRoundDuration = 6000f;
            shopDuration = 120f;
            beforeRoundDuration = 4f;
            afterRoundDuration = 4f;
            RIGGEDSPAWN = -1;
            RIGGED = false;
            INFINITERIGGED = false;
            currentRound = 0;
        }

        
    }
    void Start()
    {
        openSkillTreeCanvas.enabled = false;
    
        AudioManager.Instance.DisableMenuMusic(1f);
        if (DEBUGMODE == true)
        {
            player.stats.skillPoints = STARTING_SKILL_POINTS;
        }

        if (START_WITH_NOTHING) {
            currentRoundState = RoundStates.Nothing;
            return;
        }
        if (START_WITH_BEGIN)
        {
            StartBeforeRoundIntermission();
        } else
        {
            StartCoroutine(StartTutorialSequence());
        }

        foreach (DatabaseItemData item in viewerItemDatabase.items)
        {
            databaseItemDatas.Add(item);
        }
    }
    
    void Update()
    { 
        glitchEffectController.SetCorruptionLevel(player.stats.corruption / 100f);
        if (currentRoundState == RoundStates.Active)
        {
            if (player == null) {
                StartGameOverSequence();
                return; 
            }
            roundTimer += Time.deltaTime;

            
            if (currentRound == finalRound) {
                timeRemaining = finalRoundDuration - roundTimer;
            } else
            {
                timeRemaining = roundDuration - roundTimer;
            }

            roundManagerUI.UpdateTimer(timeRemaining);
            
            
            float trueRoundDuration = currentRound == finalRound ? finalRoundDuration : roundDuration;
            if (roundTimer >= trueRoundDuration)
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
                    } else if (obj.objectiveType == ObjectiveTypes.KillBoss)
                    {
                        obj.currentAmount = enemiesKilled;
                    }

                    
                }
            }

            if (currentRound == finalRound && currentObjectives[0].IsComplete()) EndCurrentRound();

            //enemy spawn handling
            if (Time.time - lastSpawnedEnemies > currentRoundData.spawnInterval)
            {
                if (currentRound != finalRound) {
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
            }

            // audience gift handling
            if (Time.time - lastReceivedGift > giftCooldown - player.stats.supplyCrateCooldownReduction && Time.time - lastGiftCheck > 1f && player.stats.viewers > 0f)
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
            if (currentRound == finalRound)
            {
                roundManagerUI.UpdateTimer(215940f); // basically infinite time, this is AFTER the final round btw.
                return;
            }
            roundTimer += Time.deltaTime;
            timeRemaining = shopDuration - roundTimer;
            roundManagerUI.UpdateTimer(timeRemaining);
            
            if (roundTimer >= shopDuration - 2f && !blackStartedFadingShop)
            {
                blackStartedFadingShop = true;
                BlackScreen.Instance.FadeToBlack(2f);
            }
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
                if (objectivesCompleted == currentObjectives.Count && player != null)
                {
                    StartShopSequence();
                } 
                else 
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
        } else if (currentRoundState == RoundStates.PreVictory)
        {
            roundTimer += Time.deltaTime;
            if (roundTimer >= 2f)
            {
                StartEndingSequence();
            }
        } else if (currentRoundState == RoundStates.GameVictory)
        {
            // do nothing for now.
        } else if (currentRoundState == RoundStates.Tutorial)
        {
            roundTimer += Time.deltaTime;
            // something or other, I'm not sure.
            if (currentObjectives.Count > 0 && Time.time - lastUpdatedStat > updateStatInterval)
            {
                lastUpdatedStat = Time.time;
                int completeObjectives = 0;
                foreach (Objective obj in currentObjectives)
                {
                    if (obj.IsComplete()) { completeObjectives++; continue;}
                    if (obj.objectiveType == ObjectiveTypes.Collect)
                    {
                        obj.currentAmount = orbsCollected;
                    } else if (obj.objectiveType == ObjectiveTypes.Kill)
                    {
                        obj.currentAmount = enemiesKilled;
                    } else if (obj.objectiveType == ObjectiveTypes.Parry)
                    {
                        obj.currentAmount = timesParried;
                    }  
                }
                if (completeObjectives == currentObjectives.Count && didTutorialObjectivesFlag != true)
                {
                    // hey you did all the tutorial objectives
                    didTutorialObjectivesFlag = true;
                    DialogueRoundHandler.Instance.tutorialBot.SwitchDialouge(DialogueRoundHandler.Instance.tutorialBotDialogues[0].roundDialogue[1]);
                }
            }

            
        }
    

    }
    public void AudienceGiftEvent() {
        // give the player a random item as a gift from the audience.
        Item audienceItem = viewerItemDatabase.GetAudienceItem(databaseItemDatas, player.stats.viewers, player.stats.sponsers);
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
    public IEnumerator StartTutorialSequence()
    {
        Debug.Log("Tutorial hasbegun");
        DialogueManager.Instance.EnableBG(Color.black);
        roundManagerUI.DisableTimer();
        currentRoundState = RoundStates.Tutorial;
        roundTimer = 0f;
        // Spawn Knight in the tutorial region
        yield return new WaitForSeconds(2f);
        DialogueRoundHandler.Instance.TutorialBotSpeak();
    }
    public void CreateTutorialObjectives()
    {
        ObjectiveScaling objScale1 = manualObjectiveDatabase.objectiveScalings[1]; // index 1 will be hard coded kill ojective
        Objective newObj1 = objScale1.CalculateObjective();
        currentObjectives.Add(newObj1);
        ObjectiveScaling objScale2 = manualObjectiveDatabase.objectiveScalings[2]; // index 2 will be hard coded parry obejctive
        Objective newObj2 = objScale2.CalculateObjective();
        currentObjectives.Add(newObj2);

        roundManagerUI.AddEntry(objScale1, newObj1);
        roundManagerUI.AddEntry(objScale2, newObj2);

        RoundData tRoundData = tutorialRoundDatabase.GetRoundData(1);
        List<GameObject> tempList = new();
        for (int i = 0; i < 4; i++) tempList.Add(tRoundData.enemyWeights[0].enemyPrefab);
        SpawnPlatformManager.Instance.SpawnEnemies(tempList, player.transform.position, 10);
        tempList.Clear();
        tempList.Add(tRoundData.enemyWeights[0].enemyPrefab);
        SpawnPlatformManager.Instance.SpawnEnemies(tempList, player.transform.position, 10);
    }
    public void StartBeforeRoundIntermission()
    {
        BlackScreen.Instance.FadeFromBlack(1f);
        AudioManager.Instance.PlayBattleMusic(3f);
        KnightSpawnIn();
        roundManagerUI.EnableTimer();
        player.stats.totalStyle = 0f;
        currentRoundState = RoundStates.Begin;
        roundTimer = 0f;
        Debug.Log("Round will begin soon!");

        PreSetUpRound();
        //assign the objectives for the round. 
        currentRound++;
        currentRoundData = roundDatabase.GetRoundData(currentRound);
        currentObjectives.Clear();
        roundManagerUI.ClearEntries();
        if (currentRound == finalRound)
        {
            ObjectiveScaling objScale = manualObjectiveDatabase.objectiveScalings[0]; // index 0 will be the final boss objective
            Objective newObj = objScale.CalculateObjective();
            currentObjectives.Add(newObj);
            roundManagerUI.AddEntry(objScale, newObj);
        } else
        {
            CreateNewObjectives(currentRoundData.minObjectives,currentRoundData.maxObjectives);
        }
        
    }
    public void StartNewRound()
    {
        // basic setup for a new round.
        SetUpRound();
        if (currentRound == finalRound)
        {
            List<GameObject> enemyPool = currentRoundData.enemyWeights.Select(e => e.enemyPrefab).ToList();
            List<GameObject> enemiesToSpawn = new List<GameObject>{enemyPool[0]}; // the only one will be the boss. 
            SpawnPlatformManager.Instance.SpawnEnemies(enemiesToSpawn, player.transform.position, 4); 
            // replace 4 with the actual spawn platform spawn index
        }
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
    private void PreSetUpRound()
    {
        player.stats.viewers = 0;
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
        objectivesCompleted = 0;
        initialLevel = (int)player.stats.level;


        roundManagerUI.objectiveUIManager.BringToMiddleRegular();
    }
    private void SetUpRound()
    {
        currentRoundState = RoundStates.Active;
        roundManagerUI.objectiveUIManager.TakeToProperPosition();
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

        // update the dialogue for the intermission people
        DialogueRoundHandler.Instance.HandleRoundDialogue(currentRound);

        // additional calculations
        loyalViewersGained = CalcLoyalViewersGained();
        player.stats.loyalViewers += loyalViewersGained;

        afterRoundLevel = (int)player.stats.level;
        float rep = player.stats.reputation;
        bonusPointGain = (int)(Mathf.Sqrt(highestViewersThisRound/900f) * (((rep + 100f)/500f)+1f));
        player.stats.skillPoints += bonusPointGain;
        // round summary UI popup and update 
        UpdateRoundSummaryUI();

        // disabling some UI to prepare for next phase
        BlackScreen.Instance.FadeToBlackWithDelay(afterRoundDuration - 2f, 2f);
        // statsUIManager.DisableAfterDelay(afterRoundDuration);
        roundSummaryManagerUI.DisableAfterDelay(afterRoundDuration);
        AudioManager.Instance.DisableBattleMusic(afterRoundDuration);
        if (objectivesCompleted == currentObjectives.Count)
        {
            // continue if you did all the objectives
            Debug.Log("Round " + currentRound + " ended, begin shop phase soon.");
        }



    }
    public void StartShopSequence()
    {
        BlackScreen.Instance.FadeFromBlack(2f);
        AudioManager.Instance.PlayIntermissionMusic(3f);
        blackStartedFadingShop = false;
        openSkillTreeCanvas.enabled = true;
        player.ResetRoundStats();
        
        // send player to prison so they don't kill themselves or something
        SENDTOPRISON();

        roundTimer = 0f;
        currentRoundState = RoundStates.Shop;
        Debug.Log("The shop is now open for " + shopDuration + " seconds.");
        DialogueRoundHandler.Instance.GuideBotSpeak();
    }
    public void SkipShopSequence()
    {
        if (currentRound == finalRound) return;
        openSkillTreeCanvas.enabled = false;
        roundTimer = shopDuration - 4f;
    }
    public void EndShopSequence()
    {
        skilltreeManager.DisableAfterDelay(0f);
        abilityEquipUIManager.DisableAfterDelay(0f);
        statsUIManager.EnableAfterDelay(0f);

        Debug.Log("The shop has closed.");
        openSkillTreeCanvas.enabled = false;
        AudioManager.Instance.DisableIntermissionMusic(1f);
        StartBeforeRoundIntermission();
    }
    public void StartGameOverSequence()
    {
        if ((INFINITERIGGED || JOURNALISTMODE) && player != null) {
            StartShopSequence();
            return;
        }

        BlackScreen.Instance.FadeFromBlack(2f);
        roundManagerUI.DisableTimer();
        currentRoundState = RoundStates.GameOver;
        SENDTOPRISON();

        AudioManager.Instance.PlayIntermissionMusic(2f);
        AudioManager.Instance.DisableBattleMusic(1f);

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
        if (INFINITERIGGED) {
            StartShopSequence();
            return;
        }
        BlackScreen.Instance.FadeFromBlack(2f);
        roundManagerUI.DisableTimer();
        currentRoundState = RoundStates.GameVictory;
        SENDTOPRISON();
        victoryCanvas.EnableUI("You have beaten the game! Congratulations!");
    }
    public void EndRoundSequence()
    {
        roundTimer = 0f;
        BlackScreen.Instance.FadeToBlack(2f);
        roundManagerUI.DisableTimer();
        currentRoundState = RoundStates.PreVictory;
        AudioManager.Instance.DisableIntermissionMusic(1f);
        AudioManager.Instance.PlayMenuMusic(3f);
        openSkillTreeCanvas.enabled = false;
    }
    public void StartEndingSequence()
    {
        BlackScreen.Instance.FadeFromBlack(2f);
        roundTimer = 0f;
        currentRoundState = RoundStates.GameVictory;
        FullDialogue chosenEnding = DialogueRoundHandler.Instance.GetEndingDialogue(ending);
        DialogueManager.Instance.StartFullDialogue(chosenEnding, null);
        DialogueManager.Instance.EnableBG();
        SENDTOMEGAPRISON();

    }
    public void UpdateRoundSummaryUI()
    {
        roundSummaryManagerUI.UpdateObjectives(objectivesCompleted, currentObjectives.Count);
        roundSummaryManagerUI.UpdateGrade(((StyleGrades)highestGradeThisRound).ToString());
        roundSummaryManagerUI.UpdateViewerCount((int)highestViewersThisRound);
        roundSummaryManagerUI.UpdateKills(enemiesKilled);
        roundSummaryManagerUI.UpdateParries(timesParried);
        roundSummaryManagerUI.UpdateRoundCount(currentRound);
        roundSummaryManagerUI.UpdateLoyalViewersGained(loyalViewersGained);
        roundSummaryManagerUI.UpdateRepGained(repGained);
        roundSummaryManagerUI.UpdateCorruptionGained(corruptionGained);
        roundSummaryManagerUI.UpdateLevelsGained(afterRoundLevel - initialLevel);
        roundSummaryManagerUI.UpdateBonusPointsGained(bonusPointGain);
        roundSummaryManagerUI.UpdateSkillPointsGained(afterRoundLevel - initialLevel + bonusPointGain);
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
    private void SpawnOrbs(int amt)
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
    private int CalcLoyalViewersGained()
    {
        // loyal viewers gained is 10% of total viewers this round, rounded down.
        float gain = ((player.stats.viewers - player.stats.loyalViewers) + (highestViewersThisRound - player.stats.loyalViewers))/2f;
        return Mathf.FloorToInt(gain * 0.2f);
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
        player.transform.position = PRISON.transform.position + Vector3.up * 1f;
    }
    public void GameJournalistMode(bool enable)
    {
        JOURNALISTMODE = enable;
    }
    // prison, but actually prison where the player can't do shit
    // hello, malcolm here, i didnt code this.
    private void SENDTOMEGAPRISON()
    {

    }
    
    public void MakeMidGameChoice(string choice)
    {
        midGameChoice = choice;

        // remove all other connected nodes, so we just left with the correct ones fr fr. 
        for (int i = skilltreeManager.bridgeNode.connectedNodes.Count - 1; i >= 0; i--)
        {
            if (skilltreeManager.bridgeNode.connectedNodes[i].nodeName != $"Path of {choice}")
            {
                skilltreeManager.bridgeNode.connectedNodes[i].transform.position = new Vector3(50000,50000,0);
                skilltreeManager.bridgeNode.connectedNodes.RemoveAt(i);
            }
        }

        skilltreeManager.UnlockNode(skilltreeManager.bridgeNode, true);
        skilltreeManager.MoveAllPathNodes(choice);

        if (midGameChoice == "Honor")
        {
            
            player.AddMultiplier(
                new DamageMultiplier{
                    timeCreated = Time.time,
                    type = DamageMultiplierTypes.Multiplicative,
                    amount = 0.7f,
                    lifeTime = Mathf.Infinity,
                    source = "MidGameChoice"
                }
            );
        } else if (midGameChoice == "Popularity")
        {
            // something related to popularity or something, idk. 
            
        } else if (midGameChoice == "Destruction")
        {
            player.AddMultiplier(
                new DamageMultiplier{
                    timeCreated = Time.time,
                    type = DamageMultiplierTypes.Multiplicative,
                    amount = 1.3f,
                    lifeTime = Mathf.Infinity,
                    source = "MidGameChoice"
                }
            );
        }
    }
    private void KnightSpawnIn()
    {
        if (RIGGEDSPAWN >= 0 && RIGGEDSPAWN < spawnLocationsParent.transform.childCount) {
            player.transform.position = spawnLocationsParent.transform.GetChild(RIGGEDSPAWN).position;
            return; 
        }
        player.transform.position = spawnLocationsParent.transform.GetChild(UnityEngine.Random.Range(0, spawnLocationsParent.transform.childCount)).position;
    }

    public float GetHighestViewersThisRound => highestViewersThisRound;
    public PlayerStatManager GetPlayer => player;
}

public enum RoundStates
{
    Tutorial, Active, Shop, Intermission, Begin, End, Nothing, GameOver, GameVictory, PreVictory, IntroCutscene
}