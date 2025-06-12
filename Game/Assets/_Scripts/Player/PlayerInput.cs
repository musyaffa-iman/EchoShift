using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent OnAttack;

    [Header("Input Settings")]
    [SerializeField] private bool forceMobileMode = false; // For testing mobile on desktop

    [Header("Mobile Controls")]
    [SerializeField] private FloatingJoystick movementJoystick;
    [SerializeField] private Button attackButton;

    private Vector3 mousePosition;
    private Camera mainCamera;
    private Movement movement;
    private SpriteRenderer spriteRenderer;
    private WeaponParent weaponParent;
    private Transform playerTransform;

    private Vector2 movementInput;
    private Vector2 pointerInput;

    private readonly bool isMobileDevice = Application.isMobilePlatform;
    
    private System.Action inputHandler;

    private Vector2 lastJoystickDirection = Vector2.right;

    private void Awake()
    {
        CacheComponents();
        StartCoroutine(FindMobileControlsCoroutine());
        
        bool useMobileInput = isMobileDevice || forceMobileMode;
        inputHandler = useMobileInput ? HandleMobileInput : HandleDesktopInput;
    }

    private IEnumerator FindMobileControlsCoroutine()
    {
        yield return null;
        
        if (movementJoystick == null)
        {
            movementJoystick = FindObjectOfType<FloatingJoystick>();
        }
        
        if (attackButton == null)
        {
            GameObject buttonObj = GameObject.Find("AttackButton");
            if (buttonObj != null)
            {
                attackButton = buttonObj.GetComponent<Button>();
            }
        }
        
        bool useMobile = isMobileDevice || forceMobileMode;
        
        if (useMobile)
        {
            SetupMobileControls();
        }
        
        SetMobileControlsVisibility(useMobile);
    }

    private void CacheComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        playerTransform = transform;
        movement = GetComponent<Movement>();
    }

    private void SetMobileControlsVisibility(bool visible)
    {
        if (movementJoystick != null)
        {
            movementJoystick.gameObject.SetActive(visible);
        }
        
        if (attackButton != null)
        {
            attackButton.gameObject.SetActive(visible);
        }
    }

    private void SetupMobileControls()
    {
        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(OnMobileAttackButton);
        }
    }

    private void Update()
    {
        inputHandler?.Invoke();
    }

    private void HandleMobileInput()
    {
        if (movementJoystick != null)
        {
            Vector2 joystickDirection = movementJoystick.Direction;
            
            ProcessMovementInput(joystickDirection);
            
            if (joystickDirection.magnitude > 0.1f)
            {
                lastJoystickDirection = joystickDirection.normalized;
            }
            
            Vector3 aimDirection = playerTransform.position + (Vector3)lastJoystickDirection;
            ProcessPointerInput(aimDirection);
        }
    }

    private void HandleDesktopInput()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
                if (mainCamera == null)
                {
                    Debug.LogWarning("[PlayerInput] No camera found for input handling");
                    return;
                }
            }
        }

        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        ProcessPointerInput(mousePosition);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 moveInput = new Vector2(horizontal, vertical).normalized;
        ProcessMovementInput(moveInput);

        if (Input.GetMouseButtonDown(0))
        {
            ProcessAttackInput();
        }
    }

    private void ProcessMovementInput(Vector2 moveInput)
    {
        movementInput = moveInput;
        OnMovementInput?.Invoke(movementInput);
        movement?.SetMovementInput(movementInput);
    }

    private void ProcessPointerInput(Vector2 pointerPosition)
    {
        pointerInput = pointerPosition;
        OnPointerInput?.Invoke(pointerInput);
        
        mousePosition = pointerPosition;
        FlipBasedOnMousePosition();
        if (weaponParent != null)
            weaponParent.PointerPosition = mousePosition;
    }

    private void ProcessAttackInput()
    {
        OnAttack?.Invoke();
        weaponParent?.Attack();
    }

    public void OnMobileAttackButton()
    {
        ProcessAttackInput();
    }

    private void FlipBasedOnMousePosition()
    {
        if (spriteRenderer == null || playerTransform == null) return;
        
        spriteRenderer.flipX = mousePosition.x < playerTransform.position.x;
    }

    private void OnEnable()
    {
        StartCoroutine(FindCameraCoroutine());
    }

    private IEnumerator FindCameraCoroutine()
    {
        // Wait a frame to ensure scene is loaded
        yield return null;
        
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }
}
