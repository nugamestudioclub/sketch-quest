using UnityEngine;
using UnityEngine.UI;

public class DrawingImage : MonoBehaviour {
	[field: SerializeField]
	public Image Image { get; private set; }

	[field: SerializeField]
	public string Code { get; set; }
}