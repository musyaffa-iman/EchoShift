using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Boss Components")]
    [SerializeField] private Health health;
    [SerializeField] private Animator animator;
    
    [Header("Phase Settings")]
    [SerializeField] private float phase2HealthThreshold = 0.5f;
    
    private float maxHealth;
    private bool isPhase2 = false;
    
    private SpriteRenderer spriteRenderer;
    private Transform player;
    
    public enum BossPhase { phase1, phase2 }
    public BossPhase currentPhase = BossPhase.phase1;
    
    private void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        if (health != null)
        {
            maxHealth = health.GetMaxHealth();
        }
        
        StartCoroutine(FindPlayerCoroutine());
    }
    
    private IEnumerator FindPlayerCoroutine()
    {
        while (player == null)
        {
            if (Player.CurrentPlayer != null)
            {
                player = Player.CurrentPlayer;
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
    private void Update()
    {
        if (health == null) return;
        
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            bool isInIntro = stateInfo.IsName("intro");
            health.SetInvulnerable(isInIntro);
        }
        
        CheckPhaseTransition();
        FlipSpriteTowardsPlayer();
    }
    
    private void CheckPhaseTransition()
    {
        float healthPercentage = (float)health.GetCurrentHealth() / health.GetMaxHealth();
                
        if (healthPercentage <= phase2HealthThreshold && !isPhase2)
        {
            TransitionToPhase2();
        }
    }
    
    private void TransitionToPhase2()
    {
        isPhase2 = true;
        currentPhase = BossPhase.phase2;
        
        if (animator != null)
        {
            bool hasParameter = false;
            for (int i = 0; i < animator.parameterCount; i++)
            {
                if (animator.GetParameter(i).name == "IsPhase2")
                {
                    hasParameter = true;
                    break;
                }
            }
            
            if (hasParameter)
            {
                animator.SetBool("IsPhase2", true);
            }
            else
            {
                Debug.LogError("[BossEnemy] Animator parameter 'IsPhase2' does not exist!");
            }
        }
        else
        {
            Debug.LogWarning("[BossEnemy] Animator is null! Cannot trigger phase transition");
        }
    }
    
    private void FlipSpriteTowardsPlayer()
    {
        if (spriteRenderer == null || player == null) return;
        
        float directionX = player.position.x - transform.position.x;
        
        if (directionX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(directionX), 1, 1);
        }
    }
}
