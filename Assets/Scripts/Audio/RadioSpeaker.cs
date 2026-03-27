using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

// Controls the interactable radio in a way that keeps the audio playing unless the player 'tunes in' to the broadcast, rather than resetting each interaction
// Also control a toggle for a dial light
// Uses IInteractable so player can trigger from the reticle
public class RadioSpeaker : MonoBehaviour, IInteractable
{

    [Header("Mixer Settings")] 
    
    public AudioMixer mixer;
    public float transitionTime = 1.5f;

    [Header("Visual Settings")] 
    public Light radioDialLight;
    
    private bool isTunedIn = false;

 
    void Start()
    {
        //Radio is muted when mixer is present, but it is playing
        if (mixer != null)
        {
            mixer.SetFloat("RadioInteractVol", -80f);
        }
        
        if (radioDialLight != null) radioDialLight.enabled = false;
    }

    public void Interact()
    {   
        isTunedIn = !isTunedIn;
        StopAllCoroutines();
        
        //Interaction? Yes, turn on the light
        
        if (radioDialLight != null) radioDialLight.enabled = isTunedIn;
        
        //Radio is on, tune in gradually to the radio
        if (isTunedIn)
        {
            StartCoroutine(FadeRadio(0f));
        }
        else
        {
            StartCoroutine(FadeRadio(-80f));
        }

    }

    //Calculate the fade gradually over time
    private IEnumerator FadeRadio(float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        
        mixer.GetFloat("RadioInteractVol", out currentVol);

        while (currentTime < transitionTime)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetVolume, currentTime / transitionTime);
            mixer.SetFloat("RadioInteractVol", newVol);
            yield return null;
        }
        mixer.SetFloat("RadioInteractVol", targetVolume);
    }
    
}