using System;
using UnityEngine;

[Serializable]
public struct DrawingPoint {
	public char code;
	public Transform transform;
}

public class Drawing : MonoBehaviour {
	private bool _visible;

	private string _fragment;

	[SerializeField]
	private GameObject _canvas;

	[SerializeField]
	private DrawingPoint[] _points = Array.Empty<DrawingPoint>();

	public bool Visible {
		get => _visible;
		set {
			_visible = value;
			_canvas.SetActive(_visible);
		}
	}

	void Start() {
		Visible = _visible;
	}

	private void Update() {
		var input = UnityRuntime.GameEngine.Input;
		if( Visible ) {
			if( input.IsDown(InputButton.Cancel) ) {
				Visible = false;
			}
			else {
				var button = PollDrawing(input);
				if( button != InputButton.None )
					Draw(button);
			}
		}
		else {
			if( input.IsDown(InputButton.Select) )
				Visible = true;
		}
	}

	public void Hide() {
		Visible = false;
	}

	public void Show() {
		Visible = true;
	}

	private void Draw(InputButton button) {
		if( UnityRuntime.GameEngine.TryGetCode(button, out char code) ) {
			_fragment += code;
		}
		else {
			Visible = false;
		}
	}

	private static InputButton PollDrawing(InputData input) {
		if( input.IsDown(InputButton.UpLeft) )
			return InputButton.UpLeft;
		else if( input.IsDown(InputButton.Up) )
			return InputButton.Up;
		else if( input.IsDown(InputButton.UpRight) )
			return InputButton.UpRight;
		else if( input.IsDown(InputButton.DownLeft) )
			return InputButton.DownLeft;
		else if( input.IsDown(InputButton.Down) )
			return InputButton.Down;
		else if( input.IsDown(InputButton.DownRight) )
			return InputButton.DownRight;
		else
			return InputButton.None;
	}
}