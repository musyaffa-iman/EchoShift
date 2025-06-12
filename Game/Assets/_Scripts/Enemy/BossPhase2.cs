using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase2 : StateMachineBehaviour
{
    [SerializeField] private float chaseDistance = 8f;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private float contactCooldown = 1.5f;
    
    private Transform player;
    private BossEnemy bossEnemy;
    private Movement movement;
    private WeaponParent weaponParent;
    private Collider2D bossCollider;
    private float lastContactTime;
    private bool isInCooldown = false;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        bossEnemy = animator.GetComponent<BossEnemy>();
        movement = animator.GetComponent<Movement>();
        weaponParent = animator.GetComponentInChildren<WeaponParent>();
        bossCollider = animator.GetComponent<Collider2D>();
        
        if (Player.CurrentPlayer != null)
        {
            player = Player.CurrentPlayer;
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        if (player == null)
        {
            Debug.LogWarning("[BossPhase2] Player not found!");
        }
        
        if (bossCollider != null)
        {
            bossCollider.isTrigger = true;
        }
        
        isInCooldown = false;
        lastContactTime = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;
        
        if (isInCooldown)
        {
            if (Time.time - lastContactTime >= contactCooldown)
            {
                isInCooldown = false;
            }
            else
            {
                if (movement != null)
                {
                    movement.SetMovementInput(Vector2.zero);
                }
                return;
            }
        }
        
        float distanceToPlayer = Vector2.Distance(player.position, animator.transform.position);
        
        if (distanceToPlayer < chaseDistance)
        {
            if (weaponParent != null)
            {
                weaponParent.PointerPosition = player.position;
            }
            
            Vector2 direction = (player.position - animator.transform.position).normalized;
            if (movement != null)
            {
                movement.SetMovementInput(direction);
            }
            
            CheckContactDamage(animator);
        }
        else
        {
            if (movement != null)
            {
                movement.SetMovementInput(Vector2.zero);
            }
        }
    }
    
    private void CheckContactDamage(Animator animator)
    {
        if (player == null || bossCollider == null || isInCooldown) return;
        
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider != null && bossCollider.bounds.Intersects(playerCollider.bounds))
        {
            ContactDamage();
        }
    }
    
    private void ContactDamage()
    {
        if (player == null || bossEnemy == null) return;
        
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.GetHit(contactDamage, bossEnemy.gameObject);
        }
        
        Knockback playerKnockback = player.GetComponent<Knockback>();
        if (playerKnockback != null)
        {
            GameObject knockbackSource = new GameObject("TempKnockbackSource");
            knockbackSource.transform.position = bossEnemy.transform.position;
            
            playerKnockback.PlayFeedback(knockbackSource);
            
            Object.Destroy(knockbackSource, 0.1f);
            
        }
        else
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (player.position - bossEnemy.transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
        
        isInCooldown = true;
        lastContactTime = Time.time;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (movement != null)
        {
            movement.SetMovementInput(Vector2.zero);
        }

        if (bossCollider != null)
        {
            bossCollider.isTrigger = false;
        }
        
        AudioManager.Instance.PlayMusic(AudioManager.Instance.backgroundMusic);
    }
}
