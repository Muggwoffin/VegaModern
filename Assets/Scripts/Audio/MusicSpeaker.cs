using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSpeaker : MonoBehaviour, IInteractable
{
    // Controls the vinyl player, restarting the sound effect each time the object is interacted with on/off
    //Creates a slow fade effect
    //Uses the interactable interface so the PlayerInteractor can trigger it with a raycast

    [Header("Mixer Settings")] public AudioMixerSnapshot radioOnSnapshot;
    public AudioMixerSnapshot radioOffSnapshot; //Unused for now but I may use snapshots again so keeping in at the moment
    public float transitionTime = 1.5f;
    public AudioMixer mixer;

    private bool isPlaying = false;
    private AudioSource vinylAudio;
    
    [Header("Visual Settings")] 
    public Light vinylDialLight;


    void Start()
    {
        vinylAudio = GetComponent<AudioSource>();
        mixer.SetFloat("VinylVol", -80f);
        if (vinylDialLight != null) vinylDialLight.enabled = false;
    }

    public void Interact()
    {
        //Toggle playing on or off
        isPlaying = !isPlaying;
        if (vinylDialLight != null) vinylDialLight.enabled = isPlaying;
        
        StopAllCoroutines();
        
        if (isPlaying)
        {
            vinylAudio.Play();
            StartCoroutine(FadeVinyl(0f));
        }
        else
        {
            StartCoroutine(FadeVinyl(-80f));
            Invoke("StopAudio", transitionTime);
        }
        
    }
// Creates a gradual fade over the transition time
    private IEnumerator FadeVinyl(float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        mixer.GetFloat("VinylVol", out currentVol);

        while (currentTime < transitionTime)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetVolume, currentTime / transitionTime);
            mixer.SetFloat("VinylVol", newVol);

            yield return null;
        }
        
    }
    
//Stops the audio after a fade out
    private void StopAudio()
    {
    if (!isPlaying) vinylAudio.Stop();
    }


}
