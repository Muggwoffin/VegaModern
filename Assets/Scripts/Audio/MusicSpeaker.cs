using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSpeaker : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Mixer Settings")] public AudioMixerSnapshot radioOnSnapshot;
    public AudioMixerSnapshot radioOffSnapshot;
    public float transitionTime = 1.5f;
    public AudioMixer mixer;

    private bool isPlaying = false;
    private AudioSource vinylAudio;

    void Start()
    {
        vinylAudio = GetComponent<AudioSource>();
        mixer.SetFloat("VinylVol", -80f);
    }

    public void Interact()
    {
        isPlaying = !isPlaying;
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
    

    private void StopAudio()
    {
    if (!isPlaying) vinylAudio.Stop();
    }


}
