using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Coroutine AudioCoroutine;
    private AudioSource _audioSource;

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.spatialBlend = 0;
        _audioSource.enabled = false;

        ChangeVolume(DataManager.Instance.Data.SfxVolume);
    }

    public void PlayAudio(AudioClip clip)
    {
        if (AudioCoroutine != null)
                StopCoroutine(AudioCoroutine);
            
        AudioCoroutine = StartCoroutine(PlayAudioClip(clip));
    }

    public void ChangeVolume(float value)
    {
        _audioSource.volume = ((Mathf.Log10(value) * 20)/80) * -1;
    }

    private IEnumerator PlayAudioClip(AudioClip clip)
    {
        _audioSource.enabled = true;

        _audioSource.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);

        _audioSource.enabled = false;
    }

    private void OnEnable()
    {
        DataManager.onSfxVolumeChange += ChangeVolume;
    }

    private void OnDisable()
    {
        DataManager.onSfxVolumeChange -= ChangeVolume;        
    }
}
