using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(
	fileName = nameof(GameConfig),
	menuName = "Config/" + nameof(GameConfig))
]
public class GameConfig : ScriptableObject {

	public GameObject runtime;

	public GameObject bomb;

	private AbilityCode[] _abilityCodes;

	public ReadOnlyCollection<AbilityCode> AbilityCodes => new(_abilityCodes);
}