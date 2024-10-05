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

    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] Collider2D playerCollider;

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
        if (IsFalling)
        {
            Fall();
        }
        HandleJump();
        HandleRunning();
        HandleDashing();
        
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
        else if (!Mathf.Approximately(currentXVelocity, 0))
        {
            playerBody.velocity = new Vector2(Mathf.Lerp(currentXVelocity, 0, runningFalloff), playerBody.velocity.y);
        }
    }
    
    private void OnDrawGizmosSelected()
    { 
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GroundCheckOffset(playerCollider),groundCheckRadius);
    }

    private Vector2 GroundCheckOffset(Collider2D collider2D)
    {
        
        return new Vector2(collider2D.bounds.center.x, collider2D.bounds.min.y);
    }

    private void HandleJump()
    {
        if (tryingToJump)
        {
            if (isGrounded)
            {
                Jump();
            }
            else if(UnlockedDoubleJump && !isGrounded && canDoubleJump)
            {
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
    public bool IsFalling => !IsDashing && (playerBody.velocity.y < gravityChangeSpeed || !holdingJump);
    public bool IsDashing => currentDashDuration < dashTime;
}
