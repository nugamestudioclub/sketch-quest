using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct DrawingPoint {
	public char code;
	public Transform transform;
}

public class Drawing : MonoBehaviour {
	[SerializeField]
	private float coDeltaTime = 0.01f;

	[SerializeField]
	private float fadeDuration = 0.75f;

	[SerializeField]
	private float drawingDuration = 0.5f;

	[field: SerializeField]
	public string Fragment { get; private set; } = "";

	[SerializeField]
	private Canvas _canvas;

	private readonly List<DrawingImage> _pointImages = new();

	private readonly List<DrawingImage> _edgeImages = new();

	public bool Open { get; private set; }

	public bool Visible {
		get => _canvas.gameObject.activeSelf;
		set => _canvas.gameObject.SetActive(value);
	}

	void Start() {
		foreach( var image in GetComponentsInChildren<DrawingImage>() )
			(image.Code.Length == 2 ? _edgeImages : _pointImages).Add(image);
		Visible = false;
	}

	private void Update() {
		var input = UnityRuntime.GameEngine.Input;
		if( Open ) {
			if( input.IsDown(InputButton.Select) ) {
				Hide();
			}
			else {
				var button = PollDrawing(input);
				if( button != InputButton.None )
					Draw(button);
				var (any, ability) = GameLogic.CheckAbilityCode(Fragment);
				if( !any )
					Fail();
				else if( ability != AbilityKind.None )
					Succeed();
			}
		}
		else if( !Visible ) {
			if( input.IsDown(InputButton.Select) )
				Show();
		}
	}

	public void Hide() {
		Fragment = "";
		StartCoroutine(DoHide());
	}

	public void Show() {
		StartCoroutine(DoShow());
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

	private IEnumerator DoDrawEdge(DrawingImage edge, char start) {
		var image = edge.Image;
		var code = edge.Code;
		image.fillOrigin = code[0] == start ? 0 : 1;
		float elapsedTime = 0f;
		do {
			image.fillAmount = elapsedTime / drawingDuration;
			yield return new WaitForSeconds(coDeltaTime);
			elapsedTime += coDeltaTime;
		} while( elapsedTime < fadeDuration );
		image.fillAmount = 1f;
	}

	private IEnumerator DoHide() {
		Open = false;
		if( _canvas.TryGetComponent<CanvasGroup>(out var canvasGroup) ) {
			float elapsedTime = 0f;
			do {
				canvasGroup.alpha = 1f - (elapsedTime / fadeDuration);
				yield return new WaitForSeconds(coDeltaTime);
				elapsedTime += coDeltaTime;
			} while( elapsedTime < fadeDuration );
			canvasGroup.alpha = 0f;
			Visible = false;
		}
	}

	private IEnumerator DoShow() {
		Visible = true;
		Fragment = "";
		DrawAll(_pointImages, UnityRuntime.GameEngine.InkDisabledColor);
		DrawAll(_edgeImages, Color.clear);
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
		if( _canvas.TryGetComponent<CanvasGroup>(out var canvasGroup) ) {
			float elapsedTime = 0f;
			do {
				canvasGroup.alpha = elapsedTime / fadeDuration;
				yield return new WaitForSeconds(coDeltaTime);
				elapsedTime += coDeltaTime;
			} while( elapsedTime < fadeDuration );
			canvasGroup.alpha = 1f;
		}
		Open = true;
	}

	private void Draw(InputButton button) {
		if( UnityRuntime.GameEngine.TryGetCode(button, out char end) ) {
			Fragment += end;
			var gameEngine = UnityRuntime.GameEngine;
			var defaultColor = gameEngine.InkDefaultColor;
			var disabledColor = gameEngine.InkDisabledColor;
			var hiddenColor = Color.clear;
			Debug.Log($"Points length {_pointImages.Count}");
			DrawPoints(_pointImages, Fragment, defaultColor, disabledColor);
			Debug.Log($"Edges length {_edgeImages.Count}");
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
			Debug.Log($"Draw edge '{code}'");
			image.color = secondaryColor;
			var span = pattern.Span;
			foreach( var (start, end) in GetEdges(pattern) ) {
				Debug.Log($"Draw Edge (start '{start}', end '{end}'");
				if( code.Contains(start) && code.Contains(end) ) {
					image.color = primaryColor;
					var current = span[^1];
					if( current == start || current == end )
						StartCoroutine(DoDrawEdge(edge, start));
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
		Hide();
	}

	private void Succeed() {
		Hide();
	}
}