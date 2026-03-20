using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

//Controls ambient sound zones for street, cafe and kitchen - and also potentially further sound zones
// Uses AudioMixer parameters to fade smoothly between areas when the player enters/exits them
// Called by trigger scripts so ambience follows the player's location. These are the doormats.
public class AmbienceManager : MonoBehaviour
{
    public AudioMixer mixer;
    public float fadeDuration = 2.0f;
    void Start()
    {
        // When the game starts only the street vol is audible. But other sounds are not 'off' they are playing in the background silently, this creates a persistent state of audio
        mixer.SetFloat("StreetVol", 0f);
        mixer.SetFloat("CafeVol", -80f);
        mixer.SetFloat("KitchenVol", -80f);
    }



    public void EnterCafe()
    {
        //Stop ensures there are no in-progress fades that could lead to a conflict as we transition
        StopAllCoroutines();
        StartCoroutine(FadeMixer("StreetVol", -80f, fadeDuration));
        StartCoroutine(FadeMixer("CafeVol", 0f, fadeDuration));
        StartCoroutine(FadeMixer("KitchenVol", -10f, fadeDuration));
        Debug.Log("Entered Cafe");
    }
    
    public void ExitCafe()
    {

        StopAllCoroutines();
        StartCoroutine(FadeMixer("CafeVol", -80f, fadeDuration));
        StartCoroutine(FadeMixer("StreetVol", 0f, fadeDuration));
        StartCoroutine(FadeMixer("KitchenVol", -80f, fadeDuration));
        Debug.Log("Exited Cafe");
    }

    public void EnterKitchen()
    {

        StopAllCoroutines();
        StartCoroutine(FadeMixer("KitchenVol", 0f, fadeDuration));
        StartCoroutine(FadeMixer("CafeVol", -10f, fadeDuration));
    }
    
    public void ExitKitchen()
    {

        StopAllCoroutines();
        StartCoroutine(FadeMixer("KitchenVol", -80f, fadeDuration));
        StartCoroutine(FadeMixer("CafeVol", 0f, fadeDuration));
        StartCoroutine(FadeMixer("StreetVol", -80f, fadeDuration));
    }

    //This co routine smoothly fades a single exposed parameter in the audio mixer from its current value to target dB over time
    IEnumerator FadeMixer(string param, float targetDb, float duration)
    {
        //Reads the current value
        float currentDb;
        mixer.GetFloat(param, out currentDb);
        float elapsed = 0f;

        //Graduall moves from the current dB to the targetDb over duration seconds
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newDb = Mathf.Lerp(currentDb, targetDb, elapsed / duration);
            mixer.SetFloat(param, newDb);
            yield return null;
        }
        mixer.SetFloat(param, targetDb);
    }

}
