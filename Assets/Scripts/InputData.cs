public class InputData {
	public Buttons16 _buttons;

	public bool this[InputButton button] {
		get => _buttons[(int)button - 1];
		set => _buttons[(int)button - 1] = value;
	}

	public float Horizontal { get; set; }

	public float HorizontalRaw { get; set; }

	public float Vertical { get; set; }

	public float VerticalRaw { get; set; }

	public bool IsDown(InputButton button) {
		return _buttons.IsDown((int)button - 1);
	}

	public bool IsUp(InputButton button) {
		return _buttons.IsUp((int)button - 1);
	}

	public void Press(InputButton button) {
		_buttons.Press((int)button - 1);
	}

	public void Release(InputButton button) {
		_buttons.Release((int)button - 1);
	}

	public void Update() {
		_buttons.Update();
	}
}