using System;
using System.Collections.Generic;
using UnityEngine;

public class StarsUI : MonoBehaviour {
	[SerializeField]
	private List<Animator> _countAnimators = new();

	[SerializeField]
	private Animator _separatorAnimator;

	[SerializeField]
	private List<Animator> _totalAnimators = new();

	[SerializeField]
	private int _variation = 1;

	[field: SerializeField]
	private int _count;

#if UNITY_EDITOR
	private int _previousCount;

	private void Start() {
		var gameEngine = UnityRuntime.GameEngine;
		_count = gameEngine.Stars;
		_previousCount = _count;
		Animate(gameEngine.Stars, gameEngine.MaxStars);
	}

	void Update() {
		var gameEngine = UnityRuntime.GameEngine;
		if( _count != _previousCount && _previousCount == gameEngine.Stars ) {
			gameEngine.Stars = _count;
			_previousCount = _count;
			Animate(gameEngine.Stars, gameEngine.MaxStars);
		}
	}
#endif

	public void Show(int count, int total) {
		if( _count == count )
			return;
		_count = count;
#if UNITY_EDITOR
		_previousCount = count;
#endif
		Animate(count, total);
	}

	private static void SetActive(Animator animator, bool value) {
		animator.gameObject.SetActive(value);
	}

	private void Animate(int count, int total) {
		int frame = Time.frameCount % _variation;
		int variety = 0;
		int maxDigits = _countAnimators.Count;
		int position = 0;
		foreach( int digit in GetDigits(count, maxDigits) ) {
			var animator = _countAnimators[position++];
			SetActive(animator, true);
			Play(animator, digit.ToString(), variety += 2, frame);
		}
		while( position < maxDigits ) {
			var animator = _countAnimators[position++];
			SetActive(animator, false);
		}
		Play(_separatorAnimator, "Slash", position: 0, frame);
		maxDigits = _totalAnimators.Count;
		position = 0;
		foreach( int digit in GetDigits(total, maxDigits) ) {
			var animator = _totalAnimators[position++];
			SetActive(animator, true);
			Play(animator, digit.ToString(), variety += 2, frame);
		}
		while( position < maxDigits ) {
			var animator = _totalAnimators[position++];
			SetActive(animator, false);
		}
	}

	private static IEnumerable<int> GetDigits(int value, int maxDigits) {
		if( value == 0 ) {
			yield return 0;
		}
		else {
			int position = 0;
			while( position < maxDigits && value > 0 ) {
				int digit = value % 10;
				value /= 10;
				yield return digit;
			}
		}
	}

	private void Play(Animator animator, string name, int position, int frame) {
		float length = animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length;
		int variety = (frame + position) % _variation;
		float time = variety * (length / _variation);
		animator.gameObject.SetActive(true);
		animator.Play(name, layer: 0, time);
	}
}