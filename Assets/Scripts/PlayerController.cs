using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
	[SerializeField] private float jumpHeight;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float dashSpeed;
	[SerializeField] private float dashTime = 1;
	[SerializeField] private float groundCheckRadius;
	[SerializeField, Range(0, 1)] private float runningFalloff;
	[field: SerializeField, Range(0, 360)] public float ThrowAngleDegrees { get; private set; } = 45;
	[field: SerializeField] public float ThrowPower { get; private set; }
	[field: SerializeField] public float PlatformDistance { get; private set; } = 5;

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
	[SerializeField] private float bombCooldown = 3;
	private float currentBombCooldown;
	[SerializeField] private float dashCooldown = 1.5f;
	private float currentDashCooldown;

	public bool CanDash => UnlockedDash && currentDashCooldown < 0;
	public bool CanBomb => UnlockedBomb && currentBombCooldown < 0;
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

	private float unlockedBombTime;

	private float unlockedDashTime;

	private float unlockedDoubleJumpTime;

	void Update() {
		HandleInput();
		HandleAnimation();
	}

	private void FixedUpdate() {
		HandleCooldowns();
		HandleGrounded();
		HandleRunning();

		if( IsFalling ) {
			Fall();
		}

		HandleJump();
		HandleDashing();
		HandleFacing();
		HandleBomb();

		HandleAbilityInProgress();

		ClearStaleInputs();
		ClearExpiredAbilities();
	}

	private void ClearExpiredAbilities() {
		if( unlockedBombTime <= 0 )
			UnlockedBomb = false;
		if( unlockedDashTime <= 0 )
			UnlockedDash = false;
		if( unlockedDoubleJumpTime <= 0 )
			UnlockedDoubleJump = false;
	}

	private void ClearStaleInputs() {
		tryingToDash = false;
		tryingToJump = false;
		tryingToBomb = false;
	}

	private void HandleCooldowns() {
		float deltaTime = Time.fixedDeltaTime;
		if( !CanBomb ) {
			currentBombCooldown -= deltaTime;
		}
		if( !CanDash ) {
			currentDashCooldown -= deltaTime;
		}
		return;
		var abilityDuration = UnityRuntime.GameEngine.TemporaryAbilityDuration;
		unlockedBombTime = Math.Clamp(unlockedBombTime - deltaTime, 0f, abilityDuration);
		unlockedDashTime = Math.Clamp(unlockedDashTime - deltaTime, 0f, abilityDuration);
		unlockedDoubleJumpTime = Math.Clamp(unlockedDoubleJumpTime - deltaTime, 0f, abilityDuration);
	}

	private void HandleInput() {
		var gameEngine = UnityRuntime.GameEngine;

		if( gameEngine.Paused )
			return;

		var input = gameEngine.Input;

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
	}

	private void HandleRunning() {
		float currentXVelocity = playerBody.velocity.x;
		if( IsTryingToMove ) {
			playerBody.velocity = new Vector2(moveInputDirection * moveSpeed, playerBody.velocity.y);
		}
		else if( IsMovingHorizontally ) {
			playerBody.velocity = new Vector2(Mathf.Lerp(currentXVelocity, 0, runningFalloff), playerBody.velocity.y);
		}
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(GroundCheckOffset(playerCollider), groundCheckRadius);

		//draw bomb trajectory
		Gizmos.color = Color.magenta;
		var bombEndPos = GameLogic.GetBombThrowOffset(this);
		Gizmos.DrawLine(Position, bombEndPos);
		Gizmos.DrawWireSphere(bombEndPos, groundCheckRadius);

		//draw platformSpawns
		Gizmos.color = Color.blue;
		int[] platformDegrees = { 15, 165, 210, 330 };
		foreach( var degrees in platformDegrees ) {
			Gizmos.DrawWireSphere(GameLogic.GetPlatformOffset(Position, degrees, PlatformDistance), groundCheckRadius);
		}
	}

	private Vector2 GroundCheckOffset(Collider2D value) {
		var bounds = value.bounds;
		return new Vector2(bounds.center.x, bounds.min.y + groundCheckRadius*.75f);
	}

	private void HandleJump() {
		if( tryingToJump ) {
			if( isGrounded ) {
				Jump();
				isJumping = true;
			}
			else if( UnlockedDoubleJump && !isGrounded && canDoubleJump ) {
				Jump();
				canDoubleJump = false;
				isDoubleJumping = true;
			}
		}
	}

	private void Jump() {
		float velocityY = Mathf.Max(playerBody.velocity.y, 0) + jumpHeight;
		playerBody.velocity = new Vector2(playerBody.velocity.x, velocityY);
	}

	private void Dash() {
		playerBody.gravityScale = dashingGravity;
		dashDirection = new Vector2(FacingRight ? dashSpeed : -dashSpeed, 0);
		currentDashDuration = 0;
		playerBody.velocity = dashDirection;
		currentDashCooldown = dashCooldown;
	}

	private void HandleDashing() {
		if( CanDash && tryingToDash ) {
			if( isGrounded ) {
				Dash();
			}
			else if( canAirDash ) {
				Dash();
				canAirDash = false;
			}
		}
		else if( IsDashing ) {
			playerBody.velocity = dashDirection;
			currentDashDuration += Time.fixedDeltaTime;
		}
	}
	private void Fall() {
		playerBody.gravityScale = fallingGravity;
	}

	private void HandleGrounded() {
		playerBody.gravityScale = risingGravity;
		bool wasGrounded = isGrounded;
		isGrounded = Physics2D.OverlapCircle(GroundCheckOffset(playerCollider), groundCheckRadius, ~LayerMask.GetMask("Player", "Switch"));
		if( isGrounded ) {
			canDoubleJump = true;
			canAirDash = true;
			if( !wasGrounded )
				UnityRuntime.GameEngine.AudioBank.Play(5);
		}
	}

	private void HandleFacing() {
		if( IsTryingToMove ) {
			FacingRight = moveInputDirection > 0;
			spriteRenderer.flipX = !FacingRight;
		}
	}

	private void HandleAnimation() {
		if( isJumping && !playerAnimator.IsTypePlaying(PlayerAnimationType.Jump) ) {
			playerAnimator.Play(PlayerAnimationType.Jump);
			isJumping = false;
		}
		else if( isDoubleJumping && !playerAnimator.IsTypePlaying(PlayerAnimationType.DoubleJump) ) {
			playerAnimator.Play(PlayerAnimationType.DoubleJump);
			isDoubleJumping = false;
		}
		else if( IsDashing && !playerAnimator.IsTypePlaying(PlayerAnimationType.Dash) ) {
			playerAnimator.Play(PlayerAnimationType.Dash);
		}
		else if( isGrounded && IsTryingToMove) {
			playerAnimator.Play(PlayerAnimationType.Run);
		}
		else if( isGrounded ) {
			playerAnimator.Play(PlayerAnimationType.Idle);
		}
		else if( isDoubleJumping ) {
			playerAnimator.Play(PlayerAnimationType.DoubleFall);
		}
		else if( !playerAnimator.IsTypePlaying(PlayerAnimationType.DoubleJump) && !playerAnimator.IsTypePlaying(PlayerAnimationType.Dash) ) {
			playerAnimator.Play(PlayerAnimationType.Fall);
		}
	}

	private void HandleBomb() {
		if( CanBomb && tryingToBomb ) {
			Bomb();
		}
	}

	private void Bomb() {
		var bomb = UnityRuntime.GameEngine.SpawnBomb(Position);
		UnityRuntime.GameEngine.Logic.ThrowBomb(this, bomb);
		currentBombCooldown = bombCooldown;
	}

	private void HandleAbilityInProgress() {
		var gameEngine = UnityRuntime.GameEngine;
		var abilityCode = gameEngine.AbilityInProgress;
		float abilityDuration = gameEngine.TemporaryAbilityDuration;
		switch( abilityCode.ability ) {
		case AbilityKind.Bomb:
			unlockedBombTime = abilityDuration;
			UnlockedBomb = true;
			break;
		case AbilityKind.Dash:
			unlockedDashTime = abilityDuration;
			UnlockedDash = true;
			break;
		case AbilityKind.DoubleJump:
			unlockedDoubleJumpTime = abilityDuration;
			UnlockedDoubleJump = true;
			break;
		case AbilityKind.Platform:
			HandlePlatform(abilityCode.code);
			break;
		}
		gameEngine.AbilityInProgress = default;
	}

	private void HandlePlatform(string code) {
		var gameEngine = UnityRuntime.GameEngine;
		if( gameEngine.TryGetInputButton(code[0], out var button) ) {
			gameEngine.SpawnPlatform(Position, button, PlatformDistance);
		}
	}

}