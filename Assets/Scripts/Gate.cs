using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    public bool IsOpening { get; private set; }
    public bool IsClosing { get; private set; }

    [field: SerializeField, Range(0,1)]
    public float OpenRange { get; private set; }

    [field: SerializeField]
    public float TimeToOpenSeconds { get; private set; }
    public float _currentTimeMoving; 

    [SerializeField]
    private Collider2D doorCollider;

    private Vector2 doorCenter;

    private Vector2 _openTop;
    private Vector2 _openBottom;
    private Vector2 _openMid;
    private Vector2 _closedTop;
    private Vector2 _closedBottom;
    private Vector2 _closedMid;

    public bool IsMoving => IsOpening || IsClosing;

    private void Awake()
    {
        SetOpenClosedPositions();
    }

    private void FixedUpdate()
    {
        if (IsMoving)
        {
            _currentTimeMoving += Time.deltaTime;
            if (IsOpening)
            {
                OpenRange = _currentTimeMoving / TimeToOpenSeconds;
                doorCollider.transform.position = 
                    Vector2.Lerp(_closedMid, _openMid, OpenRange);
                if (Vector2.Distance(doorCollider.transform.position,_openMid) < Mathf.Epsilon)
                {
                    IsOpen = true;
                    IsOpening = false;
                    _currentTimeMoving = 0;
                }
            } else
            {
                OpenRange = (TimeToOpenSeconds - _currentTimeMoving) / TimeToOpenSeconds;
                doorCollider.transform.position =
                    Vector2.Lerp(_closedMid, _openMid, OpenRange);
                if (Vector2.Distance(doorCollider.transform.position, _closedMid) < Mathf.Epsilon)
                {
                    IsOpen = false;
                    IsClosing = false;
                    _currentTimeMoving = 0;
                }
            }
        }
    }

    private void SetOpenClosedPositions()
    {
        Bounds doorBounds = doorCollider.bounds;
        float xPos = doorBounds.center.x;
        if (IsOpen)
        {
            
            _openTop = new Vector2(xPos, doorBounds.max.y);
            _openBottom = new Vector2(xPos, doorBounds.min.y);
            _openMid = doorBounds.center;
            _closedTop = _openBottom;
            _closedMid = new Vector2(xPos, doorBounds.min.y - doorBounds.extents.y);
            _closedBottom = new Vector2(xPos, doorBounds.min.y - doorBounds.size.y);
        }else
        {
            _openTop = new Vector2(xPos, doorBounds.max.y + doorBounds.size.y);
            _openBottom = new Vector2(xPos, doorBounds.max.y);
            _openMid = new Vector2(xPos, doorBounds.max.y + doorBounds.extents.y);
            _closedTop = _openBottom;
            _closedMid = doorBounds.center;
            _closedBottom = new Vector2(xPos, doorBounds.min.y);
        }
    }

    public void Open()
    {
        IsOpen = true;
        IsOpening = true;
    }

    public void Close()
    {
        IsOpen = false;
        IsClosing = true;
    }

    public void ToggleOpenClose()
    {
        if(IsOpen)
        {
            Close();

        }
        else
        {
            Open();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (IsOpen) Open(); else Close();
        SetOpenClosedPositions();
        Gizmos.color = Color.red;
        //draw line in closed position
        Bounds doorBounds = doorCollider.bounds;
        Vector2 startingPoint = new (doorBounds.center.x, IsOpen ? 
            doorBounds.min.y : 
            doorBounds.max.y + doorBounds.size.y);
        Vector2 endPoint = new(doorBounds.center.x, IsOpen ?
            doorBounds.min.y + doorBounds.size.y :
            doorBounds.max.y);
        Gizmos.DrawLine(startingPoint, endPoint);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startingPoint, doorBounds.extents.x);
        Gizmos.DrawWireSphere(endPoint, doorBounds.extents.x);
    }


}
