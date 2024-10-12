using UnityEngine;

public static class Extensions {
	public static int GetFrame(this AnimationClip clip, float time) {
		return (int)(time * clip.frameRate);
	}

	public static int GetFrameCount(this AnimationClip clip) {
		return (int)(clip.length * clip.frameRate);
	}
}