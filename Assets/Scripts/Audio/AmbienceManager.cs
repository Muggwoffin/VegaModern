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
        FadeController("StreetVol", -80f, fadeDuration);
        FadeController("CafeVol", 0f, fadeDuration);
        FadeController("KitchenVol", -10f, fadeDuration);
        Debug.Log("Entered Cafe");
    }
    
    public void ExitCafe()
    {

        StopAllCoroutines();
        FadeController("StreetVol", 0f, fadeDuration);
        FadeController("CafeVol", -80f, fadeDuration);
        FadeController("KitchenVol", -80f, fadeDuration);
        Debug.Log("Exited Cafe");
    }

    public void EnterKitchen()
    {

        StopAllCoroutines(); 
        FadeController("KitchenVol", 0f, fadeDuration);
       FadeController("CafeVol", -10f, fadeDuration);
    }
    
    public void ExitKitchen()
    {
        StopAllCoroutines();
       FadeController("KitchenVol", -80f, fadeDuration);
       FadeController("CafeVol", 0f, fadeDuration);
       FadeController("StreetVol", -80f, fadeDuration);
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
            float newDb = Mathf.Lerp(currentDb, targetDb, Mathf.Pow(elapsed / duration, 2f));
            mixer.SetFloat(param, newDb);
            yield return null;
        }
        mixer.SetFloat(param, targetDb);
    }
    
    // Slight refactoring to make the intent more clear and so any future changes live in one place
    private void FadeController(string parameterName, float targetDb, float duration)
    {
        StartCoroutine(FadeMixer(parameterName, targetDb, duration));
    }

}
