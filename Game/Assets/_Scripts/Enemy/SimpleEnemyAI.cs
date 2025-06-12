using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEnemyAI : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent OnAttack;
    [SerializeField] private Transform player;
    [SerializeField] private float chaseDistance = 3f, attackDistance = 0.8f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float passedTime = 1f;

    private void OnEnable()
    {
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
        if (player == null) 
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceToPlayer < chaseDistance)
        {
            OnPointerInput?.Invoke(player.position);
            if (distanceToPlayer <= attackDistance)
            {
                OnMovementInput?.Invoke(Vector2.zero);
                if (passedTime >= attackCooldown)
                {
                    passedTime = 0f;
                    OnAttack?.Invoke();
                }
            }
            else
            {
                Vector2 direction = (player.position - transform.position).normalized;
                OnMovementInput?.Invoke(direction);
            }
        }
        else
        {
            OnMovementInput?.Invoke(Vector2.zero);
        }

        if (passedTime < attackCooldown)
        {
            passedTime += Time.deltaTime;
        }
    }
}
