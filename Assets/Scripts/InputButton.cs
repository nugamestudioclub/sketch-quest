using System;

public enum InputButton {
	None,
	Select,
	Cancel,
	Jump,
	Dash,
	Bomb,
	UpLeft,
	Up,
	UpRight,
	DownLeft,
	Down,
	DownRight,
}

[Serializable]
public struct InputButtonCode {
	public InputButton button;
	public char code;
}