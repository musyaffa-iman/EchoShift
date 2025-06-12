using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    private SpriteRenderer characterSprite, weaponSprite;
    public Vector2 PointerPosition { get; set; }
    public Animator animator;
    public float delay = 0.3f;
    private bool attackCooldown;
    public bool IsAttacking { get; private set; }
    public Transform circleOrigin;
    public float radius;

    private void Start()
    {
        characterSprite = GetComponentInParent<SpriteRenderer>();
        weaponSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (IsAttacking) return;
        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        transform.right = direction;

        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -1;
        }
        else if (direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponSprite.sortingOrder = characterSprite.sortingOrder - 1;
        }
        else
        {
            weaponSprite.sortingOrder = characterSprite.sortingOrder + 1;
        }
    }

    public void Attack()
    {
        if (attackCooldown) return;
        animator.SetTrigger("Attack");
        IsAttacking = true;
        attackCooldown = true;

        if (AudioManager.Instance != null)
        {
            if (transform.parent.CompareTag("Player"))
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.playerAttack);
            }
            else if (transform.parent.CompareTag("Enemy"))
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyAttack);
            }
        }
        
        StartCoroutine(ResetAttackCooldown());
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(delay);
        attackCooldown = false;
    }

    public void ResetAttack()
    {
        IsAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            if (collider.CompareTag("Hitbox"))
            {
                // Handle living entities (players/enemies)
                Health health = collider.GetComponentInParent<Health>();
                if (health != null)
                {
                    health.GetHit(1, transform.parent.gameObject);
                }
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Props"))
            {
                // Handle destructible props
                Destructible destructible = collider.GetComponentInParent<Destructible>();
                if (destructible != null)
                {
                    destructible.TakeDamage(1, transform.parent.gameObject);
                }
            }
        }
    }
}
