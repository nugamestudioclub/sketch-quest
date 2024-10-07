using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class AudioBank : MonoBehaviour
{

    [SerializeField]
    private AudioClip[] _clips = Array.Empty<AudioClip>();

    public ReadOnlyCollection<AudioClip> Clips => new(_clips);

    [field: SerializeField]
    public int Players { get; private set; } = 5;

    private HashSet<AudioSource> _sources = new();
    private void Start()
    {
        for (int i =0; i< Players; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.volume = UnityRuntime.GameEngine.Volume;
            _sources.Add(source);
            
        }
    }

    public bool Play(int index)
    {
        foreach(AudioSource source in _sources)
        {
            if (!source.isPlaying)
            {
                source.PlayOneShot(Clips[Math.Min(index, Clips.Count-1)]);
                return true;
            }
        }
        return false;
    }
}
