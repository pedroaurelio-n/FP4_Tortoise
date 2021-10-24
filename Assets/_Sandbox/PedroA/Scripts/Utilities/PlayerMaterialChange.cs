using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterialChange : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private SkinnedMeshRenderer torso1;

    [SerializeField] private Material glideTrue;
    [SerializeField] private Material glideFalse;
    [SerializeField] private Material knockbackMaterial;

    private bool isOnKnockback;

    private void Update()
    {
        if (!isOnKnockback)
        {
            if (playerMovement.isGliding)
            {
                torso1.material = glideTrue;
            }

            else
            {
                torso1.material = glideFalse;
            }
        }
    }

    private void KnockbackMaterial(float newHealth, float maxHealth, float value)
    {
        if (value < 0)
        {
            isOnKnockback = true;
            torso1.material = knockbackMaterial;
            StartCoroutine(RevertAfterKnockback());
        }
    }

    private IEnumerator RevertAfterKnockback()
    {
        yield return new WaitForSeconds(0.4f);
        isOnKnockback = false;
    }

    private void OnEnable()
    {
        PlayerHealth.onHealthChange += KnockbackMaterial;
    }

    private void OnDisable()
    {
        PlayerHealth.onHealthChange -= KnockbackMaterial;
    }
}
