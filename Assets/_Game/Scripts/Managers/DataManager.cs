using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public delegate void SfxVolumeChange(float value);
    public static event SfxVolumeChange onSfxVolumeChange;

    public delegate void MusicVolumeChange(float value);
    public static event MusicVolumeChange onMusicVolumeChange;

    public static DataManager Instance;

    public DataHolder Data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        else
            Destroy(this);
    }

    public void ChangeSFXVolume(float value)
    {
        Data.ChangeSFXVolume(value);

        if (onSfxVolumeChange != null)
            onSfxVolumeChange(value);
    }

    public void ChangeMusicVolume(float value)
    {
        Data.ChangeMusicVolume(value);

        if (onMusicVolumeChange != null)
            onMusicVolumeChange(value);
    }

    private void OnEnable()
    {
        SliderEvent.onSfxVolumeChange += ChangeSFXVolume;
        SliderEvent.onMusicVolumeChange += ChangeMusicVolume;
    }

    private void OnDisable()
    {
        SliderEvent.onSfxVolumeChange -= ChangeSFXVolume;
        SliderEvent.onMusicVolumeChange -= ChangeMusicVolume;
    }
}
