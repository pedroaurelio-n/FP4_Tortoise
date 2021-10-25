using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UI_HealthCircle : MonoBehaviour
{
    [SerializeField] private Image healthCircle;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private float circleFillDuration;

    private float _maxHealth;
    private float _health;
    private float _circleFillDiv;

    private void Start()
    {
        healthText.text = _health.ToString();
    }

    private void UpdateHealth(float health, float maxHealth, float value)
    {
        _health = health;
        _maxHealth = maxHealth;

        _circleFillDiv = 1 / _maxHealth;

        StartCoroutine(ChangeHealthCircleFill(-value));
    }

    private IEnumerator ChangeHealthCircleFill(float value)
    {
        if (value == 0)
            yield break;

        float elapsedTime = 0;

        float currentFill = healthCircle.fillAmount;

        Debug.Log("health = " + _health);
        Debug.Log("current fill = " + currentFill);
        
        while (elapsedTime < circleFillDuration)
        {
            healthCircle.fillAmount = Mathf.Lerp(currentFill, currentFill - (_circleFillDiv * value), (elapsedTime/circleFillDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //healthCircle.fillAmount = currentFill - (part * health);

        if (_health > 0)
            healthText.text = _health.ToString();
        else
            healthText.text = "0";
        yield return null;
    }

    private void OnEnable()
    {
        PlayerHealth.onHealthChange += UpdateHealth;
    }

    private void OnDisable()
    {
        PlayerHealth.onHealthChange -= UpdateHealth;
    }
}
