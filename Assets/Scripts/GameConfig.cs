using UnityEngine;

[CreateAssetMenu(
	fileName = nameof(GameConfig),
	menuName = "Config/" + nameof(GameConfig))
]
public class GameConfig : ScriptableObject {
	[field: SerializeField]
	public GameObject Runtime { get; private set; }
}