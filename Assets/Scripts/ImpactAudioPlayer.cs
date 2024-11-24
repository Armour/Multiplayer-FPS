using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class ImpactAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource?.Play();
        Debug.Log($"Playing with {audioSource}");
    }
    private void OnDisable()
    {
        audioSource?.Stop();
    }
}
