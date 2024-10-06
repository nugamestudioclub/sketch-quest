using System;

public enum AbilityKind {
	None,
	Bomb,
	Dash,
	Jump,
	Platform,
}

[Serializable]
public struct AbilityCode {
	public AbilityKind ability;

	public string code;
}