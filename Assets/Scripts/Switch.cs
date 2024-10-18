using System;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [field:SerializeField]
    public bool IsOn { get; private set; }
    [field: SerializeField]
    public Gate Gate { get; private set; }

    [SerializeField]
    private Animator _animator;

    private void Start()
    {
        foreach (EventCollider collider in GetComponentsInChildren<EventCollider>())
        {
            collider.Collision += ColliderNode_Collision;
        }
    }

    private void ColliderNode_Collision(object sender, CollisionEventArgs e)
    {
        // Debug.Log($"Switch Activated: {gameObject.name} ({(Gate.IsOpen ? "Off -> On" : "On -> Off")})");
        _animator.Play(Gate.IsOpen ? "Off" : "On", layer: 0);
		Gate.ToggleOpenClose();
    }
}
