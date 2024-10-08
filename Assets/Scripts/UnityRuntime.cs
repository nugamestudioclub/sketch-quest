﻿using UnityEngine;

public class UnityRuntime : MonoBehaviour {
	private const string GAME_CONFIG = "GameConfig";

	private static float fixedTimeStep;

	public static GameObject Root { get; private set; }

	public static GameEngine GameEngine { get; private set; }

	private void Awake() {
		fixedTimeStep = Time.fixedDeltaTime * Time.timeScale;
		GameEngine.Awake();
	}

	void Start() {
		GameEngine.Start();
	}

	void Update() {

		var input = GameEngine.Input;
		// TODO: Replace hard-coded values with new input system action map or serialized config values

		input.Update();

		if( Input.GetKeyDown(KeyCode.Tab) )
			input.Press(InputButton.Select);
		else if( Input.GetKeyUp(KeyCode.Tab) )
			input.Release(InputButton.Select);

		if( Input.GetKeyDown(KeyCode.Space) )
			input.Press(InputButton.Jump);
		else if( Input.GetKeyUp(KeyCode.Space) )
			input.Release(InputButton.Jump);
		
		if( Input.GetKeyDown(KeyCode.E) )
			input.Press(InputButton.Bomb);
		else if( Input.GetKeyUp(KeyCode.E) )
			input.Release(InputButton.Bomb);

		if( Input.GetKeyDown(KeyCode.Q))
			input.Press(InputButton.Dash);
		else if( Input.GetKeyUp(KeyCode.Q))
			input.Release(InputButton.Dash);

		if( Input.GetKeyDown(KeyCode.Q) )
			input.Press(InputButton.UpLeft);
		else if( Input.GetKeyUp(KeyCode.Q) )
			input.Release(InputButton.UpLeft);
		if( Input.GetKeyDown(KeyCode.W) )
			input.Press(InputButton.Up);
		else if( Input.GetKeyUp(KeyCode.W) )
			input.Release(InputButton.Up);
		if( Input.GetKeyDown(KeyCode.E) )
			input.Press(InputButton.UpRight);
		else if( Input.GetKeyUp(KeyCode.E) )
			input.Release(InputButton.UpRight);
		if( Input.GetKeyDown(KeyCode.A) )
			input.Press(InputButton.DownLeft);
		else if( Input.GetKeyUp(KeyCode.A) )
			input.Release(InputButton.DownLeft);
		if( Input.GetKeyDown(KeyCode.S) )
			input.Press(InputButton.Down);
		else if( Input.GetKeyUp(KeyCode.S) )
			input.Release(InputButton.Down);
		if( Input.GetKeyDown(KeyCode.D) )
			input.Press(InputButton.DownRight);
		else if( Input.GetKeyUp(KeyCode.D) )
			input.Release(InputButton.DownRight);

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
		var timeScale = GameEngine.TimeScale;
		Time.timeScale = timeScale;
		Time.fixedDeltaTime = fixedTimeStep * timeScale;
	}

	public static float UnscaledTime(float value) {
		return value * GameEngine.TimeScale;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoad() {
		var config = Resources.Load<GameConfig>(GAME_CONFIG);
		GameEngine = new(config);
		Root = Instantiate(config.runtime);
		DontDestroyOnLoad(Root);
	}
}