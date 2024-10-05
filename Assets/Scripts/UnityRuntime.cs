using UnityEngine;

public static class UnityRuntime {
	public static GameEngine GameEngine { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoad() {
		var config = Resources.Load<GameConfig>("GameConfig");
		GameEngine = new(config);
	}
}