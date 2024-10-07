using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteHider : MonoBehaviour
{
    private void Awake()
    {
        var renderer = GetComponent<SpriteRenderer>();
        renderer.color = new(.5f,.5f,.5f, .2f);

    }
}
