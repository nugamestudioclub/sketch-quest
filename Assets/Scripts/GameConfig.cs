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

	public ReadOnlyCollection<AbilityCode> AbilityCodes => new(_abilityCodes);
}