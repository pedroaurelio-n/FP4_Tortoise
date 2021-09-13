using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlideMaterialChange : MonoBehaviour
{
    [SerializeField] private PlayerMovement2 playerMovement;
    [SerializeField] private SkinnedMeshRenderer torso1;

    [SerializeField] private Material glideTrue;
    [SerializeField] private Material glideFalse;

    private void Update()
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
