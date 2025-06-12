using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private int currentHealth, maxHealth;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;
    public UnityEvent<int> OnHealed;

    [SerializeField]
    private bool isDead = false;
    
    [SerializeField]
    private bool isInvulnerable = false;
    
    [Header("Flash Effect")]
    [SerializeField] private bool enableFlashEffect = true;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.05f;
    [SerializeField] private int flashCount = 2;
    
    [Header("Death Animation")]
    [SerializeField] private bool playDeathAnimation = false;
    [SerializeField] private string deathAnimationTrigger = "Death";
    [SerializeField] private float deathAnimationDuration = 2f;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void InitializeHealth(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;
    }

    public void GetHit(int amount, GameObject sender)
    {
        if (isDead || isInvulnerable)
            return;
        if (sender.layer == gameObject.layer)
            return;

        currentHealth -= amount;

        if (currentHealth > 0)
        {
            OnHitWithReference?.Invoke(sender);
            
            if (enableFlashEffect && spriteRenderer != null)
            {
                Flash();
            }

            if (AudioManager.Instance != null)
            {
                if (gameObject.CompareTag("Player"))
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHit);
                }
                else if (gameObject.CompareTag("Enemy"))
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHit);
                }
            }
        }
        else
        {
            OnDeathWithReference?.Invoke(sender);
            isDead = true;
            
            
            // Play death animation if enabled
            if (playDeathAnimation && animator != null)
            {
                StartCoroutine(PlayDeathAnimation());
            }
            else
            {                    
                Destroy(gameObject);
            }
        }
    }
    
    private void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }
    
    private IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
        
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }
    
    public void GetHealed(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        OnHealed?.Invoke(amount);
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsAtFullHealth()
    {
        return currentHealth >= maxHealth;
    }

    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }
    
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    private IEnumerator PlayDeathAnimation()
    {
        Debug.Log("[Health] PlayDeathAnimation coroutine started");
        
        // Stop flash effect if playing
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
        
        // Disable movement and other components during death
        DisableComponentsOnDeath();
        
        // Check if animator has the death trigger
        bool hasTrigger = false;
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.GetParameter(i).name == deathAnimationTrigger)
            {
                hasTrigger = true;
                break;
            }
        }
        
        if (hasTrigger)
        {
            Debug.Log($"[Health] Triggering death animation: {deathAnimationTrigger}");
            animator.SetTrigger(deathAnimationTrigger);
        }
        else
        {
            Debug.LogWarning($"[Health] Animator parameter '{deathAnimationTrigger}' not found!");
        }
        
        Debug.Log($"[Health] Waiting {deathAnimationDuration} seconds for death animation");
        
        // Wait for animation to complete
        yield return new WaitForSeconds(deathAnimationDuration);
        
        Debug.Log("[Health] Death animation completed, destroying GameObject");
        
        // Destroy the GameObject
        Destroy(gameObject);
    }
    
    private void DisableComponentsOnDeath()
    {
        // Disable movement
        Movement movement = GetComponent<Movement>();
        if (movement != null)
            movement.enabled = false;
            
        // Disable AI
        SimpleEnemyAI enemyAI = GetComponent<SimpleEnemyAI>();
        if (enemyAI != null)
            enemyAI.enabled = false;
            
        // Disable colliders (except triggers for visual effects)
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (!col.isTrigger)
                col.enabled = false;
        }
        
        // Disable rigidbody physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

    private void OnDestroy()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
    }
}