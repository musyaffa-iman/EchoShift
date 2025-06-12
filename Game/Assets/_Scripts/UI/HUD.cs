using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUD : MonoBehaviour
{
    [Header("Health UI")]
    public Image healthBar;
    
    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    
    [Header("References")]
    public LevelManager levelManager;
    public Health playerHealth;
    
    private GameObject lastPlayerObject;
    
    void Start()
    {
        StartCoroutine(FindPlayerCoroutine());
    }
    
    void Update()
    {
        GameObject currentPlayer = GameObject.FindWithTag("Player");
        if (currentPlayer != lastPlayerObject)
        {
            lastPlayerObject = currentPlayer;
            playerHealth = null;
            StartCoroutine(FindPlayerCoroutine());
        }
        
        UpdateHealthBar();
        if (levelManager != null)
        {
            UpdateScore(levelManager.currentScore);
        }
    }
    
    private IEnumerator FindPlayerCoroutine()
    {
        while (playerHealth == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Health health = player.GetComponent<Health>();
                if (health != null)
                {
                    playerHealth = health;
                    lastPlayerObject = player;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void UpdateHealthBar()
    {
        if (healthBar != null && playerHealth != null)
        {
            healthBar.fillAmount = (float)playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth();
        }
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
