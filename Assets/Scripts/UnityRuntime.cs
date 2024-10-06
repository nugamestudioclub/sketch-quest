using UnityEngine;

public class UnityRuntime : MonoBehaviour {
	private const string GAME_CONFIG = "GameConfig";

	private static float fixedTimeStep;

	public static GameObject Root { get; private set; }

	public static GameEngine GameEngine { get; private set; }

	[field: SerializeField]
	private float TimeScale { get; set; } = 1f;

	private void Awake() {
		TimeScale = Time.timeScale;
		fixedTimeStep = Time.fixedDeltaTime * TimeScale;
		GameEngine.Awake();
	}

	void Start() {
		GameEngine.Start();
	}

	void Update() {

		var input = GameEngine.Input;
		// TODO: Replace hard-coded values with new input system action map or serialized config values

		input.Update();

		if( Input.GetKeyDown(KeyCode.Space) )
			input.Press(InputButton.Jump);
		else if( Input.GetKeyUp(KeyCode.Space) )
			input.Release(InputButton.Jump);
		
		if( Input.GetKeyDown(KeyCode.X) )
			input.Press(InputButton.Bomb);
		else if( Input.GetKeyUp(KeyCode.X) )
			input.Release(InputButton.Bomb);

		if( Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) )
			input.Press(InputButton.Dash);
		else if( Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift) )
			input.Release(InputButton.Dash);

		input.Horizontal = Input.GetAxis("Horizontal");
		input.HorizontalRaw = Input.GetAxisRaw("Horizontal");

		input.Vertical = Input.GetAxis("Vertical");
		input.VerticalRaw = Input.GetAxisRaw("Vertical");

		GameEngine.Update(Time.deltaTime);
	}

	void FixedUpdate() {
		GameEngine.FixedUpdate(Time.fixedDeltaTime);
	}

	void LateUpdate() {
		GameEngine.LateUpdate(Time.deltaTime);
		Time.timeScale = TimeScale;
		Time.fixedDeltaTime = fixedTimeStep * TimeScale;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoad() {
		var config = Resources.Load<GameConfig>(GAME_CONFIG);
		GameEngine = new(config);
		Root = Instantiate(config.runtime);
		DontDestroyOnLoad(Root);
	}
}