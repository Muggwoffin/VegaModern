using UnityEngine;
using UnityEngine.Audio;

public class RadioSpeaker : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Mixer Settings")] public AudioMixerSnapshot radioOnSnapshot;
    public AudioMixerSnapshot radioOffSnapshot;
    public float transitionTime = 1.5f;
    
    private bool isPlaying = false;
    private AudioSource radioAudio;
 
    void Start()
    {
        radioAudio = GetComponent<AudioSource>();
        radioOffSnapshot.TransitionTo(0f);
    }

    public void Interact()
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            CancelInvoke("RadioStop");
            RadioPlay();
            radioOnSnapshot.TransitionTo(transitionTime);
        }
        else
        {
            radioOffSnapshot.TransitionTo(transitionTime);
            Invoke ("RadioStop", transitionTime);
        }
    }

    private void RadioPlay()
    {
        if (radioAudio != null) radioAudio.Play();
        Debug.Log("Playing Radio Audio");
    }
    private void RadioStop()
    {
        Debug.Log("Radio off");
        if (radioAudio != null) radioAudio.Stop();
    }


}