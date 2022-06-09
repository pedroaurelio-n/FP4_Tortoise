using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShieldMeter : MonoBehaviour
{
    [SerializeField] private PlayerShieldController shield;
    [SerializeField] private Image shieldMeter;
    private Material shieldMeterMaterial;

    private void Awake()
    {
        shieldMeterMaterial = shieldMeter.material;    
    }

    private void Update()
    {
        shieldMeterMaterial.SetFloat("_Health", shield.Health / shield.MaxHealth); // Fill Amount

        float _isOnDeflectCooldown = shield.IsOnDeflectCooldown() == true ? 0 : 1;
        shieldMeterMaterial.SetFloat("_isReflectReady", _isOnDeflectCooldown); // Cor - Deflect
    }
}
