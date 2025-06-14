using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AnimatorHandler animator;
    [Header("Score")]
    [SerializeField] private int scoreValue = 100;

    private SpriteRenderer spriteRenderer;
    private WeaponParent weaponParent;
    private Movement movement;
    private Health health;

    private Vector2 movementInput;
    private Vector2 pointerInput;

    public Vector2 MovementInput
    {
        get { return movementInput; }
        set
        {
            movementInput = value;
            if (movement != null)
                movement.SetMovementInput(value);
        }
    }

    public Vector2 PointerInput
    {
        get { return pointerInput; }
        set 
        { 
            pointerInput = value;
            if (weaponParent != null)
                weaponParent.PointerPosition = value;
            FlipBasedOnPointerPosition();
        }
    }

    public void PerformAttack()
    {
        if (weaponParent != null)
            weaponParent.Attack();
    }

    private void Awake()
    {
        weaponParent = GetComponentInChildren<WeaponParent>();
        movement = GetComponent<Movement>();
        health = GetComponent<Health>();
        
        if (health != null)
        {
            health.OnDeathWithReference.AddListener(OnEnemyDeath);
        }
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FlipBasedOnPointerPosition()
    {
        if (spriteRenderer == null) return;
        
        if (pointerInput.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void OnEnemyDeath(GameObject killer)
    {
        if (killer.CompareTag("Player"))
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.AddScore(scoreValue);
            }
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeathWithReference.RemoveListener(OnEnemyDeath);
        }
    }
}

