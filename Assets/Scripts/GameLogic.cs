using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameLogic 
{
    public void ThrowBomb(PlayerController player, Bomb bomb)
    {
        Vector2 playerVelocity = player.Velocity;
        bool playerFacingRight = player.FacingRight;
        float throwPower = player.ThrowPower;
        float throwAngleDegrees = player.ThrowAngleDegrees;
        Vector2 playerPosition = player.Position;

        float adjustedThrowAngleDegrees = playerFacingRight ? throwAngleDegrees : (throwAngleDegrees + 180) % 360;
        float adjustedThrowAngleRadians = Mathf.Deg2Rad* adjustedThrowAngleDegrees;
        Vector2 throwVector = new Vector2(math.cos(adjustedThrowAngleRadians), math.sin(adjustedThrowAngleRadians)) * throwPower;
        Vector2 finalThrowVelocity = playerVelocity + throwVector;

        bomb.Spawn(playerPosition);
        bomb.Ignite(bomb.DefaultFuseLength);
        bomb.Throw(finalThrowVelocity);

	}

	public static (bool Any, AbilityKind Result) CheckAbilityCode(ReadOnlySpan<char> fragment) {
		var abilityCodes = UnityRuntime.GameEngine.AbilityCodes;
		foreach( var abilityCode in abilityCodes ) {
			if( abilityCode.code.AsSpan(0, fragment.Length).SequenceEqual(fragment) ) {
				var matchedAbility = abilityCode.code.Length == fragment.Length
					? abilityCode.ability
					: AbilityKind.None;
				return (true, matchedAbility);
			}
		}
		return (false, AbilityKind.None);
	}
}
