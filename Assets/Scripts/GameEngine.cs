using System.Collections.ObjectModel;
using UnityEngine;

public class GameEngine {
	public InputData Input { get; } = new();
	public GameLogic Logic { get; } = new();

	public ReadOnlyCollection<AbilityCode> AbilityCodes { get; }

	public GameObject Bomb { get; private set; }
    
	public GameEngine(GameConfig config)
	{
		Bomb = GameObject.Instantiate(config.Bomb);
		AbilityCodes = config.AbilityCodes;

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