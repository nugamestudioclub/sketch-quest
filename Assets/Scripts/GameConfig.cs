using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(
	fileName = nameof(GameConfig),
	menuName = "Config/" + nameof(GameConfig))
]
public class GameConfig : ScriptableObject {
	[field: SerializeField]
	public GameObject Runtime { get; private set; }

	[field: SerializeField] public GameObject Bomb { get; private set; }

	private AbilityCode[] _abilityCodes;

	public ReadOnlyCollection<AbilityCode> AbilityCodes => new(_abilityCodes);
}