using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SliderType
{
    Music,
    Sfx
}

[CreateAssetMenu(fileName = "DataHolder", menuName = "New Data")]
public class DataHolder : ScriptableObject
{
    public float SfxVolume;
    public float MusicVolume;
    public bool FpsCounter;

    public void ChangeSFXVolume(float value)
    {
        SfxVolume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        MusicVolume = value;
    }

    public void ChangeFpsCounter(bool value)
    {
        FpsCounter = value;
    }
}
