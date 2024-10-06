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

	[SerializeField]
	private AbilityCode[] _abilityCodes = Array.Empty<AbilityCode>();
	
	public Drawing drawing;

	[SerializeField]
	private InputButtonCode[] _inputButtonCodes = Array.Empty<InputButtonCode>();

	public ReadOnlyCollection<AbilityCode> AbilityCodes => new(_abilityCodes);

	public ReadOnlyCollection<InputButtonCode> InputButtonCodes => new(_inputButtonCodes);
}