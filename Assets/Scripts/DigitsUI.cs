using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DigitsUI : MonoBehaviour {
	private readonly List<DigitAnimator> _animators = new();

	private int FrameCount => _animators.Count > 0
		? _animators[0].Animator.GetCurrentAnimatorClipInfo(0)[0].clip.GetFrameCount()
		: 0;

	void Awake() {
		foreach( var animator in GetComponentsInChildren<Animator>() )
			_animators.Add(new DigitAnimator(animator, animator.gameObject.GetComponent<Image>()));
		_animators.Reverse();
	}

	public void Show(int value) {
#if UNITY_EDITOR
		_value = value;
#endif
		int maxDigits = _animators.Count;
		int numberOfDigits = CountDigits(value);
		if( value == 0 ) {
			DrawAt(maxDigits - 1, 0);
		}
		else {
			int position = 0;
			while( position < maxDigits && value > 0 ) {
				int digit = value % 10;
				value /= 10;
				DrawAt(maxDigits - numberOfDigits + position++, digit);
			}
		}
		Hide(maxDigits - numberOfDigits);
	}

	private static int CountDigits(int value) {
		return (int)Math.Floor(Math.Log10(Math.Max(1, value)) + 1);
	}

	private void DrawAt(int position, int value) {
		if( position < 0 || position >= _animators.Count )
			return;
		var animator = _animators[position];
		animator.Show();
		int frameCount = FrameCount;
		int variety = position % (frameCount / 2);
		int currentFrame = Time.frameCount % frameCount;
		int frameOffset = currentFrame + (2 * variety);
		animator.Draw(value, frameOffset);
	}

	private void Hide(int count) {
		for( int i = 0; i < count; ++i )
			_animators[i].Hide();
	}

#if UNITY_EDITOR
	[SerializeField]
	private int _value;

	private int _previousValue = int.MinValue;

	void Update() {
		if( _value != _previousValue ) {
			Show(_value);
			_previousValue = _value;
		}
	}
#endif

}