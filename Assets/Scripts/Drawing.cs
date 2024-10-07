using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DrawingPoint {
	public char code;
	public Transform transform;
}

public class Drawing : MonoBehaviour {
	private bool _visible;

	[field: SerializeField]
	public string Fragment { get; private set; } = "";

	[SerializeField]
	private GameObject _canvas;

	private readonly List<DrawingImage> _pointImages = new();

	private readonly List<DrawingImage> _edgeImages = new();

	public bool Visible {
		get => _visible;
		set {
			if( value )
				Show();
			else
				Hide();
		}
	}

	void Start() {
		foreach( var image in GetComponentsInChildren<DrawingImage>() )
			(image.Code.Length == 2 ? _edgeImages : _pointImages).Add(image);
		Hide();
	}

	private void Update() {
		var input = UnityRuntime.GameEngine.Input;
		if( Visible ) {
			if( input.IsDown(InputButton.Select) ) {
				Visible = false;
			}
			else {
				var button = PollDrawing(input);
				if( button != InputButton.None )
					Draw(button);
				var (any, ability) = GameLogic.CheckAbilityCode(Fragment);
				Debug.Log($"any {any} ability {ability}");
				if( !any )
					Fail();
				else if( ability != AbilityKind.None )
					Succeed();
			}
		}
		else {
			if( input.IsDown(InputButton.Select) )
				Visible = true;
		}
	}

	public void Hide() {
		_visible = false;
		_canvas.SetActive(false);
		Clear();
	}

	public void Show() {
		_visible = true;
		_canvas.SetActive(true);
		var camera = Camera.main;
		if( Camera.main != null ) {
			var cameraPosition = camera.transform.position;
			var drawingPosition = _canvas.transform.position;
			_canvas.transform.position = new Vector3(
				cameraPosition.x,
				cameraPosition.y,
				drawingPosition.z
			);
		}
	}

	private static IEnumerable<(char Start, char End)> GetEdges(ReadOnlyMemory<char> chars) {
		for( int start = 0; start < chars.Length - 1; ++start ) {
			var span = chars.Slice(start, 2).Span;
			yield return (span[0], span[1]);
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

	private void Clear() {
		Fragment = "";
		DrawAll(_pointImages, UnityRuntime.GameEngine.InkDisabledColor);
		DrawAll(_edgeImages, Color.clear);
	}

	private IEnumerator DoDrawEdge(DrawingImage edge, char start, float duration) {
		var image = edge.Image;
		var code = edge.Code;
		int precision = 10;
		float delta = duration / precision;
		image.fillOrigin = code[0] == start ? 0 : 1;
		for( int i = 1; i <= precision; ++i ) {
			image.fillAmount = Mathf.Lerp(0f, 1f, (i * delta) / duration);
			yield return new WaitForSeconds(delta);
		}
	}

	private void Draw(InputButton button) {
		if( UnityRuntime.GameEngine.TryGetCode(button, out char end) ) {
			Fragment += end;
			var gameEngine = UnityRuntime.GameEngine;
			var defaultColor = gameEngine.InkDefaultColor;
			var disabledColor = gameEngine.InkDisabledColor;
			var hiddenColor = Color.clear;
			DrawPoints(_pointImages, Fragment, defaultColor, disabledColor);
			DrawEdges(_edgeImages, Fragment.AsMemory(), defaultColor, hiddenColor);
		}
		else {
			Fail();
		}
	}

	private void DrawAll(List<DrawingImage> drawingImages, Color color) {
		foreach( var drawingImage in drawingImages )
			drawingImage.Image.color = color;
	}

	private void DrawEdges(List<DrawingImage> edges, ReadOnlyMemory<char> pattern, Color primaryColor, Color secondaryColor) {
		foreach( var edge in edges ) {
			var image = edge.Image;
			var code = edge.Code;
			image.color = secondaryColor;
			var span = pattern.Span;
			foreach( var (start, end) in GetEdges(pattern) ) {
				if( code.Contains(start) && code.Contains(end) ) {
					image.color = primaryColor;
					var current = span[^1];
					if( current == start || current == end )
						StartCoroutine(DoDrawEdge(edge, start, 0.5f));
				}
			}
		}
	}

	private void DrawPoints(List<DrawingImage> points, ReadOnlySpan<char> pattern, Color primaryColor, Color secondaryColor) {
		foreach( var point in points ) {
			var image = point.Image;
			var code = point.Code;
			image.color = secondaryColor;
			foreach( var c in pattern ) {
				if( code.Contains(c) )
					image.color = primaryColor;
			}
		}
	}

	private void Fail() {
		Clear();
	}

	private void Succeed() {
		Clear();
	}
}