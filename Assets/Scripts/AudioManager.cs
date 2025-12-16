using UnityEngine;
using UnityEngine.Audio;
using System;

[System.Serializable]
public class Sound
{
    public string name;       // Name we use to call the sound (e.g. "jump")
    public AudioClip clip;    // The file you downloaded
    [Range(0f, 1f)] public float volume = 0.7f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    [HideInInspector] public AudioSource source; // The Unity speaker
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Allows us to call it from Player.cs

    public Sound[] sounds; // List of sounds

    private void Awake()
    {
        // Singleton Pattern: Ensure only one Audio Manager exists
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create an AudioSource for every sound in the list
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = false; // Don't play immediately
        }
    }

    public void PlaySFX(string name)
    {
        // Find the sound with the matching name
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound NOT Found: " + name);
            return;
        }

        s.source.Play();
    }
}