using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SliderEvent : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private SliderType sliderType;

    public Slider.SliderEvent onValueChangedOverride;

    public void Start()
    {
        switch (sliderType)
        {
            case SliderType.Music:
                mainSlider.value = DataManager.Instance.Data.MusicVolume;
                break;
            
            case SliderType.Sfx:
                mainSlider.value = DataManager.Instance.Data.SfxVolume;
                break;
        }
        
        mainSlider.onValueChanged = onValueChangedOverride;
    }
}
