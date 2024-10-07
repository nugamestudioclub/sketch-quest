using System;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class GameEngine {
	public InputData Input { get; } = new();
	public GameLogic Logic { get; } = new();

	public ReadOnlyCollection<AbilityCode> AbilityCodes { get; }

	public ReadOnlyCollection<AbilityColor> AbilityColors { get; }

	public GameObject Bomb { get; private set; }

	public GameObject Explosion { get; private set; }

    public GameObject Platform { get; private set; }

    public GameObject PlatformPoof { get; private set; }

    public Drawing Drawing { get; private set; }

	public Color InkDefaultColor { get; private set; }

	public Color InkDisabledColor { get; private set; }

	public Color InkErrorColor { get; private set; }

	public ReadOnlyCollection<InputButtonCode> InputButtonCodes { get; }

	public AbilityCode AbilityInProgress { get; set; }

	public GameEngine(GameConfig config) {
		Bomb = GameObject.Instantiate(config.bomb);
		Bomb.SetActive(false);

		Explosion = GameObject.Instantiate(config.explosion);
		Explosion.SetActive(false);

        Platform = GameObject.Instantiate(config.platform);
        Platform.SetActive(false);

        PlatformPoof = GameObject.Instantiate(config.platformPoof);
        PlatformPoof.SetActive(false);

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
        Platform.transform.parent = parent;
        PlatformPoof.transform.parent = parent;
        Drawing.transform.parent = parent;
		
	}

	public void Update(float deltaTime) {
	}

	public void FixedUpdate(float deltaTime) {
	}

	public void LateUpdate(float deltaTime) {
	}


	public Summon SpawnBomb(Vector2 location)
	{
        Summon bomb = UnityRuntime.GameEngine.Bomb.GetComponent<Summon>();
		bomb.transform.position = location;
		return bomb;
	}

	public Explosion SpawnExplosion(Vector2 location) {
		Explosion explosion = UnityRuntime.GameEngine.Explosion.GetComponent<Explosion>();
		explosion.transform.position = location;
		return explosion;
	}

    public Summon SpawnPlatform(Vector2 origin, InputButton inputButton, float distance = 0)
    {
		float degrees = inputButton switch
		{
			InputButton.UpLeft => 150,
            InputButton.UpRight => 30,
			InputButton.DownLeft => 210,
			InputButton.DownRight => 330,
            _ => -1
		};
		Vector2 offset = GameLogic.GetPlatformOffset(origin, degrees, distance);
        if (degrees < 0)
		{
			Debug.Log("less than 0 degrees");
			offset = Vector2.zero;
		}
        Summon summon = UnityRuntime.GameEngine.Platform.GetComponent<Summon>();
		summon.Spawn(origin + offset);
		summon.StartExpiring(summon.DefaultFuseLength);
        return summon;
    }

    public Explosion SpawnPlatformPoof(Vector2 location)
    {
        Explosion explosion = UnityRuntime.GameEngine.PlatformPoof.GetComponent<Explosion>();
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

    public bool TryGetInputButton(char code, out InputButton button)
    {
        var value = InputButtonCodes.FirstOrDefault(x => x.code == code);
        if (value.Equals(default))
        {
            button = InputButton.None;
            return false;
        }
        else
        {
            button = value.button;
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