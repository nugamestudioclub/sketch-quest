using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float FuseLength { get; private set; }
    public bool IsIgnited { get; private set; }

    [SerializeField] private Rigidbody2D bombBody;
    

    [field: SerializeField] public float DefaultFuseLength { get; private set; } = 3;

    private void FixedUpdate()
    {
        if (IsIgnited)
        {
            FuseLength -= Time.fixedDeltaTime;
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
        
        Debug.Log($"bomb exploded {name}");
    }

    public void Spawn(Vector2 spawnLocaton)
    {
        transform.position = spawnLocaton;
        gameObject.SetActive(true);
    }

    public void Ignite(float fuseLength)
    {
        FuseLength = fuseLength;
        IsIgnited = true;
    }

    public void Throw(Vector2 direction)
    {
        bombBody.velocity = direction;
    }
    
}
