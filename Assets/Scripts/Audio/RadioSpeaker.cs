using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class RadioSpeaker : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Mixer Settings")] 
    
    public AudioMixer mixer;
    public float transitionTime = 1.5f;

    [Header("Visual Settings")] 
    public Light radioDialLight;
    
    private bool isTunedIn = false;

 
    void Start()
    {
        if (mixer != null)
        {
            mixer.SetFloat("RadioVol", -80f);
        }
        
        if (radioDialLight != null) radioDialLight.enabled = false;
    }

    public void Interact()
    {   
        isTunedIn = !isTunedIn;
        StopAllCoroutines();
        
        if (radioDialLight != null) radioDialLight.enabled = isTunedIn;
        
        if (isTunedIn)
        {
            StartCoroutine(FadeRadio(0f));
        }
        else
        {
            StartCoroutine(FadeRadio(-80f));
        }

    }

    private IEnumerator FadeRadio(float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        
        mixer.GetFloat("RadioVol", out currentVol);

        while (currentTime < transitionTime)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetVolume, currentTime / transitionTime);
            mixer.SetFloat("RadioVol", newVol);
            yield return null;
        }
        mixer.SetFloat("RadioVol", targetVolume);
    }
    
}