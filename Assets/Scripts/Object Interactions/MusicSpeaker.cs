using UnityEngine;
using UnityEngine.Audio;

public class MusicSpeaker : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Mixer Settings")] public AudioMixerSnapshot radioOnSnapshot;
    public AudioMixerSnapshot radioOffSnapshot;
    public float transitionTime = 1.5f;

    private bool isPlaying = false;
    private AudioSource vinylAudio;

    void Start()
    {
        vinylAudio = GetComponent<AudioSource>();
        radioOffSnapshot.TransitionTo(0f);
    }

    public void Interact()
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            vinylAudio.Play();
            StartCoroutine(FadeMixerGroup("VinylVol", 0f));
        }
        else
        {
            radioOffSnapshot.TransitionTo(transitionTime);
            Invoke("RadioStop", transitionTime);
        }
    }
    

    private void StopAudio()
    {
    vinylAudio.Stop();
    }


}
