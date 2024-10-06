using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimationType
{
    Idle, Run, Jump, DoubleJump, Dash, Bomb, Platform, Fall, DoubleFall
}
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Play(PlayerAnimationType type)
    {
        animator.Play(Enum.GetNames(typeof(PlayerAnimationType))[(int)type]);
        Debug.Log($"Animation Playing: {Enum.GetNames(typeof(PlayerAnimationType))[(int)type]}");
    }

    public bool IsTypePlaying (PlayerAnimationType type)
    {
        return IsPlaying && animator.GetCurrentAnimatorStateInfo(0).IsName(Enum.GetNames(typeof(PlayerAnimationType))[(int)type]);
    }

    public bool IsPlaying => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0;
}
