using System;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Collider2D doorCollider;

    public void Open()
    {
        animator.Play("GateOpen");
    }

    public void Close()
    {
        animator.Play("GateClose");
    }

    public void ToggleOpenClose()
    {
        if(IsOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //draw line in closed position
        Bounds doorBounds = doorCollider.bounds;
        //Vector2 startingPoint = new (doorBounds, IsOpen ? doorBounds.min.y : doorBounds.min.y);
        //Gizmos.DrawLine(doorBounds.max.y, );
        //Gizmos.DrawWireSphere(GroundCheckOffset(playerCollider), groundCheckRadius);
    }
}
