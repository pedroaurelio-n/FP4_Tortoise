using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UI_HealthCircle : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private CanvasGroup healthGroup;
    [SerializeField] private Image healthCircle;
    [SerializeField] private TMP_Text healthText;

    [Header("Durations")]
    [SerializeField] private float circleFillDuration;
    [SerializeField] private float fadeStartDelay;
    [SerializeField] private float fadeDuration;

    private float _maxHealth;
    private float _health;
    private float _circleFillDiv;

    private Coroutine fadeOut;

    private void Start()
    {
        healthText.text = _health.ToString();
    }

    private void UpdateHealth(float health, float maxHealth, float value)
    {
        if (fadeOut != null)
            StopCoroutine(fadeOut);
            
        healthGroup.DOFade(1f, fadeDuration * 0.5f).OnComplete(delegate {
            _health = health;
            _maxHealth = maxHealth;

            _circleFillDiv = 1 / _maxHealth;

            StartCoroutine(ChangeHealthCircleFill(-value));
        });
    }

    private IEnumerator ChangeHealthCircleFill(float value)
    {
        //if (value == 0)
            //yield break;

        float elapsedTime = 0;

        float currentFill = healthCircle.fillAmount;
        
        while (elapsedTime < circleFillDuration)
        {
            healthCircle.fillAmount = Mathf.Lerp(currentFill, currentFill - (_circleFillDiv * value), (elapsedTime/circleFillDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (_health > 0)
            healthText.text = _health.ToString();
        else
            healthText.text = "0";

        fadeOut = StartCoroutine(HealthFadeOut());
        yield return null;
    }

    private IEnumerator HealthFadeOut()
    {
        yield return new WaitForSeconds(fadeStartDelay);

        healthGroup.DOFade(0f, fadeDuration);
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
