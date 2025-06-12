using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{
    [Header("Boss Health UI")]
    public Image healthBar;
    public GameObject healthBarContainer;
    
    [Header("References")]
    public Health bossHealth;
    
    private GameObject lastBossObject;
    
    void Start()
    {
        StartCoroutine(FindBossCoroutine());
    }
    
    void Update()
    {
        GameObject currentBoss = GameObject.FindWithTag("Enemy");
        if (currentBoss != lastBossObject)
        {
            lastBossObject = currentBoss;
            bossHealth = null;
            StartCoroutine(FindBossCoroutine());
        }
        
        UpdateHealthBar();
    }
    
    private IEnumerator FindBossCoroutine()
    {
        while (bossHealth == null)
        {
            GameObject boss = GameObject.FindWithTag("Enemy");
            if (boss != null)
            {
                Health health = boss.GetComponent<Health>();
                if (health != null)
                {
                    bossHealth = health;
                    lastBossObject = boss;
                    
                    if (healthBarContainer != null)
                    {
                        healthBarContainer.SetActive(true);
                    }        
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void UpdateHealthBar()
    {
        if (bossHealth == null)
        {
            if (healthBarContainer != null)
            {
                healthBarContainer.SetActive(false);
            }
            return;
        }
        
        // Show health bar when boss is alive
        if (healthBarContainer != null && !healthBarContainer.activeSelf)
        {
            healthBarContainer.SetActive(true);
        }
        
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)bossHealth.GetCurrentHealth() / bossHealth.GetMaxHealth();
        }
        
        // Hide health bar when boss is dead
        if (bossHealth.GetCurrentHealth() <= 0 && healthBarContainer != null)
        {
            healthBarContainer.SetActive(false);
        }
    }
}
