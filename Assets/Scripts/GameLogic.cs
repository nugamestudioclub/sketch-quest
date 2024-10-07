using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public class GameLogic 
{
    public static Vector2 GetBombThrowOffset(PlayerController player)
    {
        float throwAngleRadians = Mathf.Deg2Rad * player.ThrowAngleDegrees;
        float throwDirectionX = player.FacingRight ? math.cos(throwAngleRadians) : -math.cos(throwAngleRadians);
        float throwDirectionY = math.sin(throwAngleRadians);
        return new Vector2(throwDirectionX, throwDirectionY) * player.ThrowPower;
   
    }

    public static Vector2 GetPlatformOffset(Vector2 origin, float degrees, float distance)
    {
        float radians = Mathf.Deg2Rad * degrees;
        Vector2 offset = new Vector2(math.cos(radians), math.sin(radians)) * distance;
        return origin + offset;
    }
    public void ThrowBomb(PlayerController player, Summon bomb)
    {
        Vector2 throwVector = GetBombThrowOffset(player);
        Vector2 finalThrowVelocity = player.Velocity + throwVector;

        bomb.Spawn(player.Position);
        bomb.StartExpiring(bomb.DefaultFuseLength);
        bomb.Throw(finalThrowVelocity);

	}

	public static (bool Any, AbilityKind Result) CheckAbilityCode(ReadOnlySpan<char> fragment) {
		var abilityCodes = UnityRuntime.GameEngine.AbilityCodes;
		foreach( var abilityCode in abilityCodes ) {
            var code = abilityCode.code;
			if( code.AsSpan(0, Math.Min(code.Length, fragment.Length)).SequenceEqual(fragment) ) {
                var matchedAbility = fragment.Length >= abilityCode.code.Length
					? abilityCode.ability
					: AbilityKind.None;
				return (true, matchedAbility);
			}
		}
		return (false, AbilityKind.None);
	}
}
