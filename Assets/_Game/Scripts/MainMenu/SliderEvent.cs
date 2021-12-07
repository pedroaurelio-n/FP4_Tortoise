using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SliderEvent : MonoBehaviour
{
    public delegate void SfxVolumeChange(float value);
    public static event SfxVolumeChange onSfxVolumeChange;

    public delegate void MusicVolumeChange(float value);
    public static event MusicVolumeChange onMusicVolumeChange;

    [SerializeField] private Slider mainSlider;
    [SerializeField] private bool isMusic;

    public void Start()
    {
        if (isMusic)
            mainSlider.value = DataManager.Instance.Data.MusicVolume;
        else
            mainSlider.value = DataManager.Instance.Data.SfxVolume;
            
        mainSlider.onValueChanged.AddListener(delegate {
            if (isMusic)
            {
                if (onMusicVolumeChange != null)
                    onMusicVolumeChange(mainSlider.value);
            }
            else
            {
                if (onSfxVolumeChange != null)
                    onSfxVolumeChange(mainSlider.value);
            }
        });
    }
}
