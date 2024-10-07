using UnityEngine;
public enum SummonType
{
    Bomb,
    Platform,
}

public class Summon : MonoBehaviour
{
    public SummonType Type { get; private set; }
    public float Lifespan { get; private set; }
    public bool IsExpiring { get; private set; }

    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Animator animator;
    

    [field: SerializeField] public float DefaultFuseLength { get; private set; } = 3;

    private void FixedUpdate()
    {
        if (IsExpiring)
        {
            Lifespan -= Time.fixedDeltaTime;

            if (Lifespan <= 0)
            {
                Expire();
            }
        }
    }

    public void Expire()
    {
        gameObject.SetActive(false);
        IsExpiring = false;
        //summon explosion
        if (Type == SummonType.Bomb)
        {
            Explosion explosion = UnityRuntime.GameEngine.SpawnExplosion(transform.position);
            explosion.Spawn(transform.position, explosion.DefaultDetonationLength);
        }
        else
        {

        }
        
    }

    public void Spawn(Vector2 spawnLocaton)
    {
        transform.position = spawnLocaton;
        gameObject.SetActive(true);
        animator.Play("BombIdle");
    }

    public void Ignite(float fuseLength)
    {
        Lifespan = fuseLength;
        IsExpiring = true;
        animator.Play("BombTicking");
    }

    public void Throw(Vector2 direction)
    {
        body.velocity = direction;
    }
    
}
