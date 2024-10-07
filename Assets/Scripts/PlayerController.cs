using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpHeight;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime = 1;
    [SerializeField] private float groundCheckRadius;
    [SerializeField, Range(0, 1)] private float runningFalloff;
    [field:SerializeField, Range(0, 360)] public float ThrowAngleDegrees { get; private set; } = 45;
    [field:SerializeField] public float ThrowPower { get; private set; }
    
    private float moveInputDirection;
    private bool tryingToJump;
    private bool tryingToDash;
    private bool tryingToBomb;
    private bool holdingJump;
    private bool isGrounded = false;
    private bool canDoubleJump;
    private bool canAirDash;
    private float currentDashDuration = float.MaxValue;
    private Vector2 dashDirection;
    private bool isJumping;
    private bool isDoubleJumping;

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerAnimation playerAnimator;

    [SerializeField] private float fallingGravity;
    [SerializeField] private float risingGravity;
    [SerializeField] private float dashingGravity;
    [SerializeField] private float gravityChangeSpeed;

    [field: SerializeField] public bool UnlockedDoubleJump { get; set; }
    [field: SerializeField] public bool UnlockedDash { get; set; }
    [field: SerializeField] public bool UnlockedBomb { get; set; }
    private bool IsTryingToMove => !Mathf.Approximately(moveInputDirection, 0);

    private bool IsMovingHorizontally => !Mathf.Approximately(playerBody.velocity.x, 0);
    public bool IsFalling => !IsDashing && (playerBody.velocity.y < gravityChangeSpeed || !holdingJump);
    public bool IsDashing => currentDashDuration < dashTime;
    public Vector2 Velocity => playerBody.velocity;
    public Vector2 Position => transform.position;
    public bool FacingRight { get; private set; }

    void Update()
    {
        var input = UnityRuntime.GameEngine.Input;

        moveInputDirection = input.HorizontalRaw;
        holdingJump = input[InputButton.Jump];

		if( !tryingToJump ) {
            tryingToJump = input.IsDown(InputButton.Jump);
		}

		if( !tryingToDash ) {
            tryingToDash = input.IsDown(InputButton.Dash);
		}
        
        if( !tryingToBomb ) {
            tryingToBomb = input.IsDown(InputButton.Bomb);
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
        HandleBomb();

        ClearStaleInputs();
    }

    private void ClearStaleInputs()
    {
        tryingToDash = false;
        tryingToJump = false;
        tryingToBomb = false;
    }
    private void HandleRunning()
    {
        float currentXVelocity = playerBody.velocity.x;
        if (IsTryingToMove)
        {
            playerBody.velocity = new Vector2(moveInputDirection * moveSpeed, playerBody.velocity.y);
        }
        else if (IsMovingHorizontally)
        {
            playerBody.velocity = new Vector2(Mathf.Lerp(currentXVelocity, 0, runningFalloff), playerBody.velocity.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GroundCheckOffset(playerCollider), groundCheckRadius);
    }

    private Vector2 GroundCheckOffset(Collider2D value)
    {
        var bounds = value.bounds;
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
            else if (UnlockedDoubleJump && !isGrounded && canDoubleJump)
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
        playerBody.gravityScale = dashingGravity;
        dashDirection = new Vector2(FacingRight ? dashSpeed : -dashSpeed, 0);
        currentDashDuration = 0;
        playerBody.velocity = dashDirection;
    }

    private void HandleDashing()
    {
        if (UnlockedDash && tryingToDash)
        {
            if (isGrounded)
            {
                Dash();
            }
            else if (canAirDash)
            {
                Dash();
                canAirDash = false;
            }
        }
        else if (IsDashing)
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
        isGrounded = Physics2D.OverlapCircle(GroundCheckOffset(playerCollider), groundCheckRadius, LayerMask.GetMask("Ground"));
        if (isGrounded)
        {
            canDoubleJump = true;
            canAirDash = true;
        }
    }

    private void HandleFacing()
    {
        if (IsTryingToMove)
        {
            FacingRight = moveInputDirection > 0;
            spriteRenderer.flipX = !FacingRight;
        }
    }

    private void HandleAnimation()
    {
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
    }

    private void HandleBomb()
    {
        if (UnlockedBomb && tryingToBomb)
        {
            Bomb();
        }
    }

    private void Bomb()
    {
        var bomb = UnityRuntime.GameEngine.SpawnBomb(Position);
        UnityRuntime.GameEngine.Logic.ThrowBomb(this, bomb);
    }
    
}
