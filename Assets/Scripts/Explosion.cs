
using UnityEngine;

public class Explosion : MonoBehaviour
{

    private float detonationDuration;
    [SerializeField] private Animator animator;


    [field: SerializeField] public float DefaultDetonationLength { get; private set; } = .5f;

    private void FixedUpdate()
    {

        detonationDuration -= Time.fixedDeltaTime;
        if (detonationDuration <= 0)
        {
            Expire();
        }
    }

    public void Expire()
    {
        gameObject.SetActive(false);
    }

    public void Spawn(Vector2 spawnLocaton, float duration)
    {
        detonationDuration = duration;
        transform.position = spawnLocaton;
        gameObject.SetActive(true);
        animator.Play("BombExplode");
    }

}
