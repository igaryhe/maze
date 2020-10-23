using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] sounds;
    public Sound[] footstep;

    private void Awake()
    {
        // base.Awake();
        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = true;
        }
    }

    public void Play(int n)
    {
        sounds[n].source.Play();
    }

    public void Stop(int n)
    {
        sounds[n].source.Stop();
    }

    private void Start()
    {
        // Play(0);
        // Play(1);
        // Play(2);
    }
}
