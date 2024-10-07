using UnityEngine;
public enum SummonType
{
    Bomb,
    Platform,
}

public class Summon : MonoBehaviour
{
    [field: SerializeField]
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
        var gameEngine = UnityRuntime.GameEngine;
		//summon explosion
		if (Type == SummonType.Bomb)
        {
            Explosion explosion = UnityRuntime.GameEngine.SpawnExplosion(transform.position);
            explosion.Spawn(transform.position, explosion.DefaultDetonationLength);
            gameEngine.AudioBank.Play(gameEngine.Random.Next(11, 14));
        }
        else
        {
            Explosion explosion = UnityRuntime.GameEngine.SpawnPlatformPoof(transform.position);
            explosion.Spawn(transform.position, explosion.DefaultDetonationLength);
			gameEngine.AudioBank.Play(gameEngine.Random.Next(6, 10));
		}

    }

    public void Spawn(Vector2 spawnLocaton)
    {
        transform.position = spawnLocaton;
        gameObject.SetActive(true);
        if (animator)
            animator.Play("BombIdle");
    }

    public void StartExpiring(float fuseLength)
    {
        Lifespan = fuseLength;
        IsExpiring = true;
        if (animator)
            animator.Play("BombTicking");
    }

    public void Throw(Vector2 direction)
    {
        body.velocity = direction;
    }

}
