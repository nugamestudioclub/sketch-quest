using System;
using UnityEngine;

public enum AbilityKind {
	None,
	Bomb,
	Dash,
	DoubleJump,
	Platform,
}

[Serializable]
public struct AbilityCode {
	public AbilityKind ability;

	public string code;
}

[Serializable]
public struct AbilityColor {
	public AbilityKind ability;
	public Color color;
}