
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    private float detonationDuration;
    [SerializeField] private Animator animator;

    private CircleCollider2D col2d;
    [field: SerializeField] public float DefaultDetonationLength { get; private set; } = .5f;

    private HashSet<EventCollider> colliders = new();
    private void Awake()
    {
        col2d = GetComponent<CircleCollider2D>();
    }
    private void FixedUpdate()
    {

        detonationDuration -= Time.fixedDeltaTime;
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, col2d.radius *col2d.gameObject.transform.localScale.x, LayerMask.GetMask("Switch"));
        if (hitCollider)
        {
            EventCollider eventCol = hitCollider.GetComponent<EventCollider>();
            colliders.Add(eventCol);
            eventCol.Collide(hitCollider);
        }
        
        if (detonationDuration <= 0)
        {
            Expire();
        }
    }

    public void Expire()
    {
        foreach(var collider in colliders)
        {
            collider.UnCollide(col2d);
        }
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
