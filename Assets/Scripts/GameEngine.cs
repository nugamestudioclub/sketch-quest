using UnityEngine;

public class GameEngine {
	public InputData Input { get; } = new();
	public GameLogic Logic { get; } = new();
	public GameObject Bomb { get; private set; }
    
	public GameEngine(GameConfig config)
	{
		Bomb = GameObject.Instantiate(config.Bomb);

	}
    
	public void Awake() {

	}

	public void Start() {

	}

	public void Update(float deltaTime) {
	}

	public void FixedUpdate(float deltaTime) {
	}

	public void LateUpdate(float deltaTime) {
	}
}