using UnityEngine;

public class RadioSpeaker : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private bool isPlaying = false;
    private AudioSource radioAudio;
    void Start()
    {
        radioAudio = GetComponent<AudioSource>();
    }

    public void Interact()
    {
       isPlaying = !isPlaying;
       if (isPlaying)
       {
           RadioPlay();
       }
       else
       {
           RadioStop();
       }
    }

    private void RadioStop()
    {
        Debug.Log("Radio off");
        if (radioAudio != null) radioAudio.Stop();
    }

    private void RadioPlay()
    {
        if (radioAudio != null) radioAudio.Play();
        Debug.Log("Playing Radio Audio");
    }
}
