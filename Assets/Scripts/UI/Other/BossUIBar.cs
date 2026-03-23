using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class BossUIBar : StatsUIBar
{
    public static BossUIBar Instance;
    public GameObject laggingBar;
    public TextMeshProUGUI bossName;
    public TextMeshProUGUI damageIndicator;
    public EnemyStatManager enemy;
    private CanvasGroup canvasGroup;
    private float currentDmgAmount = 0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
    private Coroutine ResetDmgCoroutine;
    private IEnumerator ResetDmgBar()
    {
        canvasGroup.alpha = 1f;
        damageIndicator.alpha = 1f;
        yield return new WaitForSeconds(4f);
        currentDmgAmount = 0f;
        damageIndicator.alpha = 0f;
    }
    private IEnumerator DisableBarAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        canvasGroup.alpha = 0f;
    }
    public void AttachEnemyToBar(EnemyStatManager enemy)
    {
        canvasGroup.alpha = 1f;
        if (this.enemy != null)
        {
            this.enemy.OnTakeDamage.RemoveListener(OnTakeDamage);
        }

        this.enemy = enemy;
        this.enemy.OnTakeDamage.AddListener(OnTakeDamage);
        this.enemy.OnDeath.AddListener(OnDeath);
        bossName.text = enemy.name;
        UpdateBar(Mathf.Max(0,enemy.stats.currentHP), enemy.stats.maxHP);
        damageIndicator.alpha = 0f;
        
    }
    public void OnDeath()
    {
        DisableBarAfterDelay(5f);
        enemy.OnDeath.RemoveListener(OnDeath);
    }
    public void OnTakeDamage(float damage)
    {
        UpdateBar(Mathf.Max(0,enemy.stats.currentHP), enemy.stats.maxHP);
        currentDmgAmount+=damage;
        damageIndicator.text = $"{Mathf.FloorToInt(currentDmgAmount)}";

        if (ResetDmgCoroutine != null)
        {
            StopCoroutine(ResetDmgCoroutine);
            ResetDmgCoroutine = null;
        }

        ResetDmgCoroutine = StartCoroutine(ResetDmgBar());
    }
}