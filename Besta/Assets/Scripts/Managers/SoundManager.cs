using UnityEngine;

public class SoundManager
{
    public AudioSource managerAudioSource;
    public void Init()
    {
        GameObject go = GameObject.Find("@Manager");
        managerAudioSource = go.GetComponent<AudioSource>();
        if (managerAudioSource == null)
        {
            managerAudioSource = go.AddComponent<AudioSource>();
        }
        managerAudioSource.loop = false;
        managerAudioSource.playOnAwake = false;
        managerAudioSource.volume = 0.3f;
    }
}
