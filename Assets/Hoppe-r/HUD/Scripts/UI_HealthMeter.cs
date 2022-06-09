using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthMeter : MonoBehaviour
{
    [SerializeField] private GameObject healthContainer;
    [SerializeField] private GameObject healthBackgroundContainer;
    [SerializeField] private GameObject healthIconPrefab;
    [SerializeField] private GameObject healthIconEmptyPrefab;

    private List<GameObject> icons = new List<GameObject>();

    private void UpdateHealth(float health, float maxHealth, float value = 1)
    {
        if (value == 0)
        {
            InstantiateIcons(maxHealth);
        }

        else
        {            
            for (int i = 0; i < maxHealth; i++)
            {
                if (i < health)
                    icons[i].transform.gameObject.SetActive(true);
                else
                    icons[i].transform.gameObject.SetActive(false);
            }
        }
    }

    private void InstantiateIcons(float maxHealth)
    {
        if (icons.Count != 0)
        {
            for (int i = icons.Count - 1; i <= 0; i--)
            {
                Destroy(icons[i].gameObject);
                icons.RemoveAt(i);
            }
        }

        for (int i = 0; i < maxHealth; i++)
        {
            var icon = Instantiate(healthIconPrefab, healthContainer.transform);
            icons.Add(icon);

            var iconBackground = Instantiate(healthIconEmptyPrefab, healthBackgroundContainer.transform);
        }
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
