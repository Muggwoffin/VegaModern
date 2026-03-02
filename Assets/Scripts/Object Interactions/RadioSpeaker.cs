using UnityEngine;
using UnityEngine.Audio;

public class RadioSpeaker : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Mixer Settings")] public AudioMixerSnapshot radioOnSnapshot;
    public AudioMixerSnapshot radioOffSnapshot;
    public float transitionTime = 1.5f;
    
    private bool isTunedIn = false;

 
    void Start()
    {
        radioOffSnapshot.TransitionTo(0f);
    }

    public void Interact()
    {   
        isTunedIn = !isTunedIn;
        if (isTunedIn)
        {
            radioOnSnapshot.TransitionTo(transitionTime);
        }
        else
        {
            radioOffSnapshot.TransitionTo(transitionTime);
        }
    }
    
}