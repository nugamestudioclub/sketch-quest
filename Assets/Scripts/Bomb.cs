using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float FuseLength { get; private set; }
    public bool IsIgnited { get; private set; }

    [SerializeField] private Rigidbody2D bombBody;
    [SerializeField] private Animator animator;
    

    [field: SerializeField] public float DefaultFuseLength { get; private set; } = 3;

    private void FixedUpdate()
    {
        if (IsIgnited)
        {
            FuseLength -= Time.fixedDeltaTime;
            Debug.Log($"FuseLength: {FuseLength}");

            if (FuseLength <= 0)
            {
                Explode();
            }
        }
    }

    public void Explode()
    {
        gameObject.SetActive(false);
        IsIgnited = false;
        //summon explosion

        Explosion explosion = UnityRuntime.GameEngine.SpawnExplosion(transform.position);
        explosion.Spawn(transform.position, explosion.DefaultDetonationLength);
        Debug.Log($"bomb exploded {name}");
    }

    public void Spawn(Vector2 spawnLocaton)
    {
        transform.position = spawnLocaton;
        gameObject.SetActive(true);
        animator.Play("BombIdle");
        Debug.Log($"currentTime spawn: {Time.fixedTime}");
    }

    public void Ignite(float fuseLength)
    {
        FuseLength = fuseLength;
        IsIgnited = true;
        animator.Play("BombTicking");
    }

    public void Throw(Vector2 direction)
    {
        bombBody.velocity = direction;
    }
    
}
