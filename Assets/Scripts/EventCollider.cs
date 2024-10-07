using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionEventArgs : EventArgs
{
    public string type;

    public CollisionEventArgs(string type)
    {
        this.type = type;
    }
}
[RequireComponent(typeof(Collider2D))]
public class EventCollider : MonoBehaviour
{
    [field: SerializeField]
    public string Type { get; set; }

    public event EventHandler<CollisionEventArgs> Collision;

    private HashSet<Collider2D> colliders = new();

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Collide(Collider2D collision)
    {
        if (!colliders.Contains(collision))
        {
            colliders.Add(collision);

            OnCollision(new CollisionEventArgs(Type));

        }
    }

    public void UnCollide(Collider2D collision)
    {
        colliders.Clear();
        if (colliders.Contains(collision))
        {
            colliders.Remove(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Collide(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        UnCollide(collision);
    }


    protected virtual void OnCollision(CollisionEventArgs e)
    {
        Collision?.Invoke(this, e);
    }
}
