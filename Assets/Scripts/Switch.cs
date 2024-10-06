using System;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool IsOn { get; private set; }
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
        if (!IsOn)
        {
            Gate.Open();
        }
        
    }
}
