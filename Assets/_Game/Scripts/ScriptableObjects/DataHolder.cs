using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataHolder", menuName = "New Data")]
public class DataHolder : ScriptableObject
{
    public float SfxVolume;
    public float MusicVolume;

    public void ChangeSFXVolume(float value)
    {
        SfxVolume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        MusicVolume = value;
    }
}
