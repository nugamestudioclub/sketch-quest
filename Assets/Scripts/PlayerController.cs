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
    private bool isJumping;
    private bool isDoubleJumping;

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
        HandleAnimation();
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
            playerBody.velocity = new Vector2(moveInputDirection * moveSpeed, playerBody.velocity.y);
        }
        else if (IsMovingHorizontally)
        {
            playerBody.velocity = new Vector2(Mathf.Lerp(currentXVelocity, 0, runningFalloff), playerBody.velocity.y);
        }
    }

    private bool IsMovingHorizontally => !Mathf.Approximately(playerBody.velocity.x, 0);
    
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
                Jump();
                isJumping = true;
            }
            else if(UnlockedDoubleJump && !isGrounded && canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
                isDoubleJumping = true;
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

    private void HandleFacing()
    {
        float currentXVelocity = playerBody.velocity.x;
        if (!Mathf.Approximately(currentXVelocity, 0))
        {
            facingRight = currentXVelocity > 0;
            spriteRenderer.flipX = !facingRight;
        }
    }

    private void HandleAnimation()
    {
        //if you have activated jump, wait for that animation to complete - do nothing
        if (isJumping && !playerAnimator.IsTypePlaying(PlayerAnimationType.Jump))
        {
            playerAnimator.Play(PlayerAnimationType.Jump); 
            isJumping = false; 
        }
        else if (isDoubleJumping && !playerAnimator.IsTypePlaying(PlayerAnimationType.DoubleJump))
        {
            playerAnimator.Play(PlayerAnimationType.DoubleJump);
            isDoubleJumping = false; 
        }
        else if (IsDashing && !playerAnimator.IsTypePlaying(PlayerAnimationType.Dash))
        {
            playerAnimator.Play(PlayerAnimationType.Dash);
        }
        else if (isGrounded && IsMovingHorizontally)
        {
            playerAnimator.Play(PlayerAnimationType.Run);
        }
        else if (isGrounded)
        {
            playerAnimator.Play(PlayerAnimationType.Idle);
        }
        else if (isDoubleJumping)
        {
            playerAnimator.Play(PlayerAnimationType.DoubleFall);
        }
        else if (!playerAnimator.IsTypePlaying(PlayerAnimationType.DoubleJump) && !playerAnimator.IsTypePlaying(PlayerAnimationType.Dash))
        {
            playerAnimator.Play(PlayerAnimationType.Fall);
        }
        
        //if activate double jump, do nothing until it is complete
        //if activate dash, do nothing until it is complete
        
        //otherwise
        
        //if you're grounded and not moving, idle 
        //if you're grounded and moving, run
        //if you're not grounded and doublejump happened, play doublejump fall
        
        //otherwise
        
        //play fall 
    }
    
    
    
    
    public bool IsFalling => !IsDashing && (playerBody.velocity.y < gravityChangeSpeed || !holdingJump);
    public bool IsDashing => currentDashDuration < dashTime;
}
