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
    [SerializeField] private float fillStartDelay;
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
        _circleFillDiv = 1 / maxHealth;

        if (value == 0)
        {
            _health = value;
            StartCoroutine(FillHealthCircleToMax(maxHealth));
        }

        else
        {
            if (fadeOut != null)
                StopCoroutine(fadeOut);
                
            healthGroup.DOFade(1f, fadeDuration * 0.5f).OnComplete(delegate {
                _health = health;
                _maxHealth = maxHealth;

                StartCoroutine(ChangeHealthCircleFill(-value));
            });
        }
    }

    private IEnumerator FillHealthCircleToMax(float maxHealth)
    {
        yield return new WaitForSeconds(fillStartDelay);

        _maxHealth = maxHealth;

        while (_health < _maxHealth)
        {
            float elapsedTime = 0;

            float currentFill = healthCircle.fillAmount;

            _health++;

            while (elapsedTime < circleFillDuration * 0.75f)
            {
                healthCircle.fillAmount = Mathf.Lerp(currentFill, currentFill + _circleFillDiv, (elapsedTime/circleFillDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            healthCircle.fillAmount = _circleFillDiv * _health;

            healthText.text = _health.ToString();

            yield return null;
        }

        fadeOut = StartCoroutine(HealthFadeOut());
        yield return null;
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

        healthCircle.fillAmount = _circleFillDiv * _health;

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
