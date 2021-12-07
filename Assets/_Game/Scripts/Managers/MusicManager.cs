using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private List<AudioClip> musicClips = default;
    [SerializeField] private bool isLoopAwake;
    private AudioSource _audioSource;

    private bool isMusicPlaying;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isMusicPlaying = true;

        _audioSource = GetComponent<AudioSource>();

        ChangeVolume(DataManager.Instance.Data.MusicVolume);

        _audioSource.clip = musicClips[0];

        if (isLoopAwake)
        {
            _audioSource.Play();
        }
    }

    private void Update()
    {
        if (isLoopAwake)
            return;

        if (isMusicPlaying)
        {
            _audioSource.enabled = false;
            isMusicPlaying = false;
        }

        else if (!isMusicPlaying)
        {
            _audioSource.enabled = true;
            isMusicPlaying = true;
            _audioSource.Play();
        }
    }

    public void ChangeVolume(float value)
    {
        _audioSource.volume = ((Mathf.Log10(value) * 20)/80) * -1;
    }

    public void ChangeSong(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void OnEnable()
    {
        DataManager.onMusicVolumeChange += ChangeVolume;
    }

    private void OnDisable()
    {
        DataManager.onMusicVolumeChange -= ChangeVolume;        
    }
}
