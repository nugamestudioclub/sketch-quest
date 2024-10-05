using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PlayIdle()
    {
        animator.Play("Idle");
    }
    
    public void PlayRun()
    {
        animator.Play("Run");
    }
    
    public void PlayJump()
    {
        animator.Play("Jump");
    }
    
    public void PlayDoubleJump()
    {
        animator.Play("DoubleJump");
    }
    
    public void PlayDash()
    {
        animator.Play("Dash");
    }
    
    public void PlayBomb()
    {
        animator.Play("Bomb");
    }
    
    public void PlayPlatform()
    {
        animator.Play("Platform");
    }

    public void PlayFall()
    {
        var animationInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!animationInfo.IsName("Jump") && !animationInfo.IsName("DoubleJump") && !animationInfo.IsName("DoubleFall"))
        {
            animator.Play("Fall");
        }
    }
    
}
