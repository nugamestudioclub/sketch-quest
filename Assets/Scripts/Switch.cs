using System;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [field:SerializeField]
    public bool IsOn { get; private set; }
    [field: SerializeField]
    public Gate Gate { get; private set; }

    private void Start()
    {
        foreach (EventCollider collider in GetComponentsInChildren<EventCollider>())
        {
            collider.Collision += ColliderNode_Collision;
        }
    }

    private void ColliderNode_Collision(object sender, CollisionEventArgs e)
    {
        Debug.Log($"Switch Activated: {gameObject.name}");
        Gate.ToggleOpenClose();
        
    }
}
