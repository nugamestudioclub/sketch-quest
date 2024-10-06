using System;
using System.Collections.ObjectModel;
using UnityEngine;

public class GameEngine {
	public InputData Input { get; } = new();
	public GameLogic Logic { get; } = new();

	public ReadOnlyCollection<AbilityCode> AbilityCodes { get; }

	public GameObject Bomb { get; private set; }

    public GameObject Explosion { get; private set; }

	public Drawing Drawing { get; private set; }

	public ReadOnlyCollection<InputButtonCode> InputButtonCodes { get; }

	public GameEngine(GameConfig config)
	{
		Bomb = GameObject.Instantiate(config.bomb);
		Bomb.SetActive(false);

        Explosion = GameObject.Instantiate(config.explosion);
        Explosion.SetActive(false);

        AbilityCodes = config.AbilityCodes;

		InputButtonCodes = config.InputButtonCodes;
	}

	public void Awake() {
        
    }

	public void Start() {
        var parent = UnityRuntime.Root.transform;
        Bomb.transform.parent = parent;
        Explosion.transform.parent = parent;
    }

	public void Update(float deltaTime) {
		if( Input.IsDown(InputButton.Select) ) {

		}
	}

	public void FixedUpdate(float deltaTime) {
	}

	public void LateUpdate(float deltaTime) {
	}

	public Bomb SpawnBomb(Vector2 location)
	{
        Bomb bomb = UnityRuntime.GameEngine.Bomb.GetComponent<Bomb>();
		bomb.transform.position = location;
		return bomb;
    }

    public Explosion SpawnExplosion(Vector2 location)
    {
        Explosion explosion = UnityRuntime.GameEngine.Explosion.GetComponent<Explosion>();
        explosion.transform.position = location;
        return explosion;
    }

	public bool TryGetCode(InputButton button, out char code) {
		var value = new InputButtonCode { button = button };
		var index = InputButtonCodes.IndexOf(value);
		if( index >= 0 ) {
			code = InputButtonCodes[index].code;
			return true;
		}
		else {
			code = default;
			return false;
		}
	}
}