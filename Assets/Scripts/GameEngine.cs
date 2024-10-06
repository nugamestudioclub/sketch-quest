using System.Collections.ObjectModel;
using UnityEngine;

public class GameEngine {
	public InputData Input { get; } = new();
	public GameLogic Logic { get; } = new();

	public ReadOnlyCollection<AbilityCode> AbilityCodes { get; }

	public GameObject Bomb { get; private set; }

    public GameObject Explosion { get; private set; }

    public GameEngine(GameConfig config)
	{

		Bomb = GameObject.Instantiate(config.bomb);
		Bomb.SetActive(false);
        Explosion = GameObject.Instantiate(config.explosion);
        Explosion.SetActive(false);
        AbilityCodes = config.AbilityCodes;

	}

	public void Awake() {
        
    }

	public void Start() {
        var parent = UnityRuntime.Root.transform;
        Bomb.transform.parent = parent;
        Explosion.transform.parent = parent;
    }

	public void Update(float deltaTime) {
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


}