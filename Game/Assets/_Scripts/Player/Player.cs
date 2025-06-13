using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AnimatorHandler animator;

    private SpriteRenderer spriteRenderer;
    public static Player Instance { get; private set; }
    public static Transform CurrentPlayer { get; private set; }
    private WeaponParent weaponParent;
    private Health playerHealth;

    private Vector2 movementInput;
    private Vector2 pointerInput;

    public Vector2 MovementInput
    {
        get { return movementInput; }
        set { movementInput = value; }
    }

    public Vector2 PointerInput
    {
        get { return pointerInput; }
        set { pointerInput = value; }
    }

    public void PerformAttack()
    {
        if (weaponParent != null)
            weaponParent.Attack();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        weaponParent = GetComponentInChildren<WeaponParent>();
        CurrentPlayer = transform;
        
        playerHealth = GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.OnDeathWithReference.AddListener(OnPlayerDied);
        }
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnPlayerDied(GameObject deadPlayer)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnPlayerDeath();
        }
        else
        {
            Debug.LogError("[Player] Could not find LevelManager to notify of player death!");
        }
    }

    private void OnDestroy()
    {
        if (CurrentPlayer == transform)
            CurrentPlayer = null;
            
        if (playerHealth != null)
        {
            playerHealth.OnDeathWithReference.RemoveListener(OnPlayerDied);
        }
    }
}

