public struct Buttons16 {
	public bool this[int input] {
		readonly get => ((Value >> (2 * input)) & 0b01) != 0;
		set {
			if( value )
				Press(input);
			else
				Release(input);
		}
	}

	public uint Value { get; set; }

	public readonly bool IsDown(int input) {
		return ((Value >> (2 * input)) & 0b11) == 0b01;
	}

	public readonly bool IsUp(int input) {
		return ((Value >> (2 * input)) & 0b11) == 0b10;
	}

	public void Press(int input) {
		Value |= (uint)(0b01 << (2 * input));
	}

	public void Release(int input) {
		Value &= ~(uint)(0b01 << (2 * input));
	}

	public void Update() {
		var present = Value & 0b01010101010101010101010101010101;
		var past = present << 1;
		Value = past | present;
	}
}