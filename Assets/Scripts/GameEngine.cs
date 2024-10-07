using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class GameEngine {
	public InputData Input { get; } = new();
	public GameLogic Logic { get; } = new();

	public ReadOnlyCollection<AbilityCode> AbilityCodes { get; }

	public ReadOnlyCollection<AbilityColor> AbilityColors { get; }

	public GameObject Bomb { get; private set; }

	public GameObject Explosion { get; private set; }

	public Drawing Drawing { get; private set; }

	public Color InkDefaultColor { get; private set; }

	public Color InkDisabledColor { get; private set; }

	public Color InkErrorColor { get; private set; }

	public ReadOnlyCollection<InputButtonCode> InputButtonCodes { get; }

	public GameEngine(GameConfig config) {
		Bomb = GameObject.Instantiate(config.bomb);
		Bomb.SetActive(false);

		Explosion = GameObject.Instantiate(config.explosion);
		Explosion.SetActive(false);

		AbilityCodes = config.AbilityCodes;
		AbilityColors = config.AbilityColors;

		InkDefaultColor = config.inkDefaultColor;
		InkDisabledColor = config.inkDisabledColor;
		InkErrorColor = config.inkErrorColor;

		InputButtonCodes = config.InputButtonCodes;

		Drawing = GameObject.Instantiate<Drawing>(config.drawing);

	}

	public void Awake() {

	}

	public void Start() {
		var parent = UnityRuntime.Root.transform;
		Bomb.transform.parent = parent;
		Explosion.transform.parent = parent;
		Drawing.transform.parent = parent;
	}

	public void Update(float deltaTime) {
	}

	public void FixedUpdate(float deltaTime) {
	}

	public void LateUpdate(float deltaTime) {
	}

	public Bomb SpawnBomb(Vector2 location) {
		Bomb bomb = UnityRuntime.GameEngine.Bomb.GetComponent<Bomb>();
		bomb.transform.position = location;
		return bomb;
	}

	public Explosion SpawnExplosion(Vector2 location) {
		Explosion explosion = UnityRuntime.GameEngine.Explosion.GetComponent<Explosion>();
		explosion.transform.position = location;
		return explosion;
	}

	public bool TryGetCode(InputButton button, out char code) {
		var value = InputButtonCodes.FirstOrDefault(x => x.button == button);
		if( value.Equals(default) ) {
			code = default;
			return false;
		}
		else {
			code = value.code;
			return true;
		}
	}

	public bool TryGetColor(AbilityKind ability, out Color color) {
		var value = AbilityColors.FirstOrDefault(x => x.ability == ability);
		if( value.Equals(default) ) {
			color = Color.clear;
			return false;
		}
		else {
			color = value.color;
			return true;
		}
	}
}