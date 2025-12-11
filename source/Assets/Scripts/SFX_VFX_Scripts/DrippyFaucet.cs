using UnityEngine;
using System.Collections;

public class DrippyFaucet : MonoBehaviour
{
    [Header("Drip Settings")]
    public float minDelay;
    public float maxDelay;
    
    [Header("Feedback")]
    public AudioSource audioSource;
    public AudioClip[] dripSounds;
    
    private Coroutine dripRoutine;
    
    private void OnEnable()
    {
        dripRoutine = StartCoroutine(DripLoop());
    }

    private void OnDisable()
    {
        if (dripRoutine != null)
            StopCoroutine(dripRoutine);
    }
    
    private IEnumerator DripLoop()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            
            PlaySound();
        }
    }
    
    private void PlaySound()
    {
        if (audioSource != null && dripSounds.Length == 0)
            return;
        
        int index = Random.Range(0, dripSounds.Length);
        
        audioSource.PlayOneShot(dripSounds[index]);
    }
}
