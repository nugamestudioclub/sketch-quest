using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpHeight; 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime = 1;
    [SerializeField] private float groundCheckRadius;
    [SerializeField, Range(0,1)] private float runningFalloff;

    private float moveInputDirection;
    private bool tryingToJump;
    private bool tryingToDash;
    private bool holdingJump;
    private bool isGrounded = false;
    private bool canDoubleJump;
    private bool canAirDash;
    private float currentDashDuration = float.MaxValue;
    private Vector2 dashDirection;
    private bool facingRight;

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerAnimation playerAnimator;

    [SerializeField] private float fallingGravity;
    [SerializeField] private float risingGravity;
    [FormerlySerializedAs("gravityChangeHeight")] [SerializeField] private float gravityChangeSpeed;

    [field:SerializeField] bool UnlockedDoubleJump { get; set; }
    [field:SerializeField] bool UnlockedDash { get; set; }
    private bool tryingToMove => !Mathf.Approximately(moveInputDirection, 0);
    
    void Update()
    {
        moveInputDirection = Input.GetAxisRaw("Horizontal");
        holdingJump = Input.GetKey(KeyCode.Space);
        
        if (!tryingToJump)
        {
            tryingToJump = Input.GetKeyDown(KeyCode.Space);
        }

        if (!tryingToDash)
        {
            tryingToDash = Input.GetKeyDown(KeyCode.LeftShift);
        }
    }

    private void FixedUpdate()
    {
        HandleGrounded();
        HandleRunning();
        
        if (IsFalling)
        {
            Fall();
        }
        
        HandleJump();
        HandleDashing();
        HandleFacing();
        
        tryingToDash = false;
        tryingToJump = false;
    }

    private void HandleRunning()
    {
        float currentXVelocity = playerBody.velocity.x;
        if(tryingToMove)
        {
            if (isGrounded)
            {
                playerAnimator.PlayRun();
            }
            playerBody.velocity = new Vector2(moveInputDirection * moveSpeed, playerBody.velocity.y);
        }
        else if (!Mathf.Approximately(currentXVelocity, 0))
        {
            if (isGrounded)
            {
                playerAnimator.PlayIdle();
            }
            playerBody.velocity = new Vector2(Mathf.Lerp(currentXVelocity, 0, runningFalloff), playerBody.velocity.y);
        }
    }
    
    private void OnDrawGizmosSelected()
    { 
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GroundCheckOffset(playerCollider),groundCheckRadius);
    }

    private Vector2 GroundCheckOffset(Collider2D bullshit)
    {
        var bounds = bullshit.bounds;
        return new Vector2(bounds.center.x, bounds.min.y);
    }

    private void HandleJump()
    {
        if (tryingToJump)
        {
            if (isGrounded)
            {
                playerAnimator.PlayJump();
                Jump();
            }
            else if(UnlockedDoubleJump && !isGrounded && canDoubleJump)
            {
                playerAnimator.PlayDoubleJump();
                Jump();
                canDoubleJump = false; 
            }
        }
    }
    private void Jump()
    {
        float velocityY = Mathf.Max(playerBody.velocity.y, 0) + jumpHeight;
        playerBody.velocity = new Vector2(playerBody.velocity.x, velocityY);
    }
    
    private void Dash()
    {
        playerBody.gravityScale = 0;
        dashDirection = new Vector2(moveInputDirection * dashSpeed, 0);
        
        if (Mathf.Approximately(moveInputDirection, 0))
        {
            dashDirection = new Vector2(facingRight? dashSpeed: -dashSpeed, 0);
        }
        currentDashDuration = 0;
        
        playerBody.velocity = dashDirection;
    }

    private void HandleDashing()
    {
        if(UnlockedDash && tryingToDash)
        {
            if (isGrounded)
            {
                Dash();
            }
            else if(canAirDash)
            {
                Dash();
                canAirDash = false;
            }
        }
        else if(IsDashing)
        {
            playerBody.velocity = dashDirection;
            currentDashDuration += Time.fixedDeltaTime;
        }
    }
    private void Fall()
    {
        playerAnimator.PlayFall();
        playerBody.gravityScale = fallingGravity;
    }

    private void HandleGrounded()
    {
        playerBody.gravityScale = risingGravity;
        isGrounded =Physics2D.OverlapCircle(GroundCheckOffset(playerCollider), groundCheckRadius, LayerMask.GetMask("Ground"));
        if (isGrounded)
        {
            canDoubleJump = true;
            canAirDash = true;  
        }
    }

    public void HandleFacing()
    {
        float currentXVelocity = playerBody.velocity.x;
        if (!Mathf.Approximately(currentXVelocity, 0))
        {
            facingRight = currentXVelocity > 0;
            spriteRenderer.flipX = !facingRight;
        }
    }
    
    public bool IsFalling => !IsDashing && (playerBody.velocity.y < gravityChangeSpeed || !holdingJump);
    public bool IsDashing => currentDashDuration < dashTime;
}
