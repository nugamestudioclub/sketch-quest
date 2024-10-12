using UnityEngine;
using UnityEngine.UI;

public readonly struct DigitAnimator {
	public Animator Animator { get; }
	public Image Image { get; }

	public DigitAnimator(Animator animator, Image image) {
		Animator = animator;
		Image = image;
	}

	public void Draw(int value, int frameOffset = 0) {
		if( Animator == null )
			return;
		string name = value.ToString();
		int layer = 0;
		float time = frameOffset * Animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;
		Animator.Play(name, layer, time);
	}

	public void Show() {
		if( Image == null )
			return;
		Image.enabled = true;
	}

	public void Hide() {
		if( Image == null )
			return;
		Image.enabled = false;
	}
}