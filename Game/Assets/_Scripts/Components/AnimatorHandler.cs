using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Movement movement;

    public void Update()
    {
        Vector2 moveInput = movement.MoveInput;

        if (animator != null)
        {
            if (moveInput != Vector2.zero)
            {
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
    }

    public void FlipSprite(float directionX)
    {
        if (directionX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(directionX), 1, 1);
        }
    }
}

