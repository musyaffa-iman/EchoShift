using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase1 : StateMachineBehaviour
{
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private float shootingCooldown = 1.5f;
    [SerializeField] private Transform shootingPoint;
    
    private Transform player;
    private BossEnemy bossEnemy;
    private ObjectPool bulletPool;
    private float lastShootTime;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossEnemy = animator.GetComponent<BossEnemy>();
        
        if (BulletPoolManager.Instance != null)
        {
            bulletPool = BulletPoolManager.Instance.GetBulletPool();
        }
        
        if (shootingPoint == null)
        {
            Transform shootingPointChild = animator.transform.Find("ShootingPoint");
            if (shootingPointChild != null)
            {
                shootingPoint = shootingPointChild;
            }
            else
            {
                shootingPoint = animator.transform;
                Debug.LogWarning("[BossPhase1] ShootingPoint not found, using boss center for bullet spawn");
            }
        }
        
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
            Debug.LogWarning("[BossPhase1] Player not found!");
        }
        
        if (bulletPool == null)
        {
            Debug.LogWarning("[BossPhase1] Bullet pool not found!");
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || bulletPool == null) return;
        
        float distanceToPlayer = Vector2.Distance(player.position, animator.transform.position);
        Movement movement = animator.GetComponent<Movement>();
        
        if (distanceToPlayer <= shootingRange)
        {
            if (movement != null)
            {
                movement.SetMovementInput(Vector2.zero);
            }
            
            if (Time.time - lastShootTime >= shootingCooldown)
            {
                ShootAtPlayer(animator);
                lastShootTime = Time.time;
            }
        }
        else
        {
            if (movement != null)
            {
                Vector2 direction = (player.position - animator.transform.position).normalized;
                movement.SetMovementInput(direction);
            }
        }
    }
    
    private void ShootAtPlayer(Animator animator)
    {
        if (player == null || bulletPool == null || shootingPoint == null) return;
        
        GameObject bullet = bulletPool.GetFromPool();
        if (bullet != null)
        {
            // Calculate direction first
            Vector2 direction = (player.position - shootingPoint.position).normalized;
            
            // Position bullet slightly forward from shooting point to prevent sticking
            Vector3 spawnPosition = shootingPoint.position + (Vector3)(direction * 0.5f);
            bullet.transform.position = spawnPosition;
            
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(direction, animator.gameObject, bulletPool);
            }
        }
        
        animator.SetTrigger("Attack");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
