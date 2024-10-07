using System;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(
	fileName = nameof(GameConfig),
	menuName = "Config/" + nameof(GameConfig))
]
public class GameConfig : ScriptableObject {
	public GameObject runtime;

	public GameObject bomb;

    public GameObject explosion;

    public GameObject platform;

	public GameObject platformPoof;

	public GameObject music;

    [SerializeField]
	private AbilityCode[] _abilityCodes = Array.Empty<AbilityCode>();
	
	public Drawing drawing;

	[SerializeField]
	private InputButtonCode[] _inputButtonCodes = Array.Empty<InputButtonCode>();

	[SerializeField]
	private AbilityColor[] _abilityColors = Array.Empty<AbilityColor>();

	public Color inkDefaultColor;

	public Color inkDisabledColor;

	public Color inkErrorColor;

	public ReadOnlyCollection<AbilityCode> AbilityCodes => new(_abilityCodes);

	public ReadOnlyCollection<InputButtonCode> InputButtonCodes => new(_inputButtonCodes);

	public ReadOnlyCollection<AbilityColor> AbilityColors => new(_abilityColors);

	public float pausedTimeScale = 0.25f;
}