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
	[SerializeField]
	private float coDeltaTime = 0.01f;

	[SerializeField]
	private float fadeDuration = 0.75f;

	[SerializeField]
	private float drawingDuration = 0.5f;

	[SerializeField]
	private float hideDelay = 0.75f;

	[field: SerializeField]
	public string Fragment { get; private set; } = "";

	[SerializeField]
	private Canvas _canvas;

	private readonly List<DrawingImage> _pointImages = new();

	private readonly List<DrawingImage> _edgeImages = new();

	public bool Open { get; private set; }

	public bool Finished { get; private set; }

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
		if( Finished )
			return;
		var gameEngine = UnityRuntime.GameEngine;
		var input = gameEngine.Input;
		if( Visible ) {
			if( Open && input.IsDown(InputButton.Select) ) {
				Hide();
			}
			else {
				var button = PollDrawing(input);
				if( button != InputButton.None )
					Draw(button);
				var (any, ability) = GameLogic.CheckAbilityCode(Fragment);
				if( !any )
					Fail();
				else if( ability != AbilityKind.None ) {
					var abilityInProgress = new AbilityCode() {
						ability = ability,
						code = Fragment
					};
					gameEngine.AbilityInProgress = abilityInProgress;
					Succeed(abilityInProgress);
				}

			}
		}
		else {
			if( !Open && input.IsDown(InputButton.Select) )
				Show();
		}
	}

    private void LateUpdate()
    {
        var camera = Camera.main;
        if (camera != null)
        {
            var cameraPosition = camera.transform.position;
            var drawingPosition = _canvas.transform.position;
            _canvas.transform.position = new Vector3(
                cameraPosition.x,
                cameraPosition.y,
                drawingPosition.z
            );
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
		float deltaTime = UnityRuntime.UnscaledTime(coDeltaTime);
		float duration = UnityRuntime.UnscaledTime(drawingDuration);
		do {
			image.fillAmount = elapsedTime / duration;
			yield return new WaitForSeconds(deltaTime);
			elapsedTime += deltaTime;
		} while( elapsedTime < duration );
		image.fillAmount = 1f;
	}

	private IEnumerator DoHide() {
		Open = false;
		var gameEngine = UnityRuntime.GameEngine;
		gameEngine.Paused = false;
		if( _canvas.TryGetComponent<CanvasGroup>(out var canvasGroup) ) {
			float elapsedTime = 0f;
			float deltaTime = coDeltaTime;
			float duration = fadeDuration;
			do {
				canvasGroup.alpha = 1f - (elapsedTime / duration);
				yield return new WaitForSeconds(deltaTime);
				elapsedTime += deltaTime;
			} while( elapsedTime < duration );
			canvasGroup.alpha = 0f;
		}
		Visible = false;
		Finished = false;
	}

	IEnumerator DoFail() {
		yield return new WaitForSeconds(UnityRuntime.UnscaledTime(drawingDuration));
		var gameEngine = UnityRuntime.GameEngine;
		var errorColor = gameEngine.InkErrorColor;
		var disabledColor = gameEngine.InkDisabledColor;
		var hiddenColor = Color.clear;
		DrawPoints(_pointImages, Fragment.AsSpan(), errorColor, disabledColor);
		DrawEdges(_edgeImages, Fragment.AsMemory(), errorColor, hiddenColor);
		yield return new WaitForSeconds(UnityRuntime.UnscaledTime(hideDelay));
		Hide();
	}

	IEnumerator DoSucceed(AbilityCode abilityInProgress) {
		yield return new WaitForSeconds(UnityRuntime.UnscaledTime(drawingDuration));
		var gameEngine = UnityRuntime.GameEngine;
		gameEngine.TryGetColor(abilityInProgress.ability, out var abilityColor);
		var disabledColor = gameEngine.InkDisabledColor;
		var hiddenColor = Color.clear;
		DrawPoints(_pointImages, Fragment.AsSpan(), abilityColor, disabledColor);
		DrawEdges(_edgeImages, Fragment.AsMemory(), abilityColor, hiddenColor);
		yield return new WaitForSeconds(UnityRuntime.UnscaledTime(hideDelay));
		Hide();
	}

	private IEnumerator DoShow() {
		var gameEngine = UnityRuntime.GameEngine;
		gameEngine.Paused = true;
		Finished = false;
		Visible = true;
		Fragment = "";
		DrawAll(_pointImages, gameEngine.InkDisabledColor);
		DrawAll(_edgeImages, Color.clear);
		
		if( _canvas.TryGetComponent<CanvasGroup>(out var canvasGroup) ) {
			float elapsedTime = 0f;
			float deltaTime = UnityRuntime.UnscaledTime(coDeltaTime);
			float duration = UnityRuntime.UnscaledTime(fadeDuration);
			do {
				canvasGroup.alpha = elapsedTime / duration;
				yield return new WaitForSeconds(deltaTime);
				elapsedTime += deltaTime;
			} while( elapsedTime < duration );
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
			DrawPoints(_pointImages, Fragment, defaultColor, disabledColor);
			DrawEdges(_edgeImages, Fragment.AsMemory(), defaultColor, hiddenColor, last: true);
		}
		else {
			Fail();
		}
	}

	private void DrawAll(List<DrawingImage> drawingImages, Color color) {
		foreach( var drawingImage in drawingImages )
			drawingImage.Image.color = color;
	}

	private void DrawEdges(List<DrawingImage> edges, ReadOnlyMemory<char> pattern, Color primaryColor, Color secondaryColor, bool last = false) {
		foreach( var edge in edges ) {
			var image = edge.Image;
			var code = edge.Code;
			image.color = secondaryColor;
			var span = pattern.Span;
			foreach( var (start, end) in GetEdges(pattern) ) {
				if( code.Contains(start) && code.Contains(end) ) {
					image.color = primaryColor;
					var current = span[^1];
					if( last && (current == start || current == end) )
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
		Finished = true;
		StartCoroutine(DoFail());
	}

	private void Succeed(AbilityCode abilityInProgress) {
		Finished = true;
		StartCoroutine(DoSucceed(abilityInProgress));
	}
}