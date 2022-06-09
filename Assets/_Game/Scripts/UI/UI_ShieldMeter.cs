using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace aa
{
    public class UI_ShieldMeter : MonoBehaviour
    {
        [SerializeField] private PlayerShieldController shield;
        [SerializeField] private Image shieldMeter;
        [SerializeField] private Image shieldDeflect;

        private void Start()
        {
            shieldMeter.color = Color.cyan;
            shieldDeflect.color = Color.cyan;
        }

        private void Update()
        {
            HandleMeterColor();
            HandleDeflectColor();

            shieldMeter.fillAmount = shield.Health / shield.MaxHealth;
        }

        private void HandleMeterColor()
        {
            if (shield.IsShieldBroken())
                shieldMeter.color = Color.gray;
            else
                shieldMeter.color = Color.cyan;
        }

        private void HandleDeflectColor()
        {
            if (shield.IsOnDeflectCooldown())
                shieldDeflect.color = Color.grey;
            else
                shieldDeflect.color = Color.cyan;
        }
    }
}
