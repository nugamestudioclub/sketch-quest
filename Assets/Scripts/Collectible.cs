using UnityEngine;

public enum CollectibleKind {
	None,
	Star,
	WinGame,
}

public class Collectible : MonoBehaviour {
	[field: SerializeField]
	public CollectibleKind Kind { get; private set; }

	public bool Collected { get; private set; }

	private void Start() {
		foreach( EventCollider collider in GetComponentsInChildren<EventCollider>() ) {
			collider.Collision += ColliderNode_Collision;
		}
	}

	private void ColliderNode_Collision(object sender, CollisionEventArgs e) {
		if( Collected )
			return;
		Collected = true;
		var gameEngine = UnityRuntime.GameEngine;
		switch( Kind ) {
		case CollectibleKind.Star:
			gameEngine.AudioBank.Play(14);
			++gameEngine.Stars;

			break;
		case CollectibleKind.WinGame:
			TransitionManager.ToCredits();
			break;
		}
		Destroy(gameObject);
	}
}