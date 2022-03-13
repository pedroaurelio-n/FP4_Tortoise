using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TA_BakeNavMeshSurface : TriggerAction
{
    [SerializeField] private NavMeshSurface[] surfaces;
    [SerializeField] private float delayInterval;

    protected override void ActivateAction()
    {
        StartCoroutine(BakeNavMeshSurfaces());
    }

    private IEnumerator BakeNavMeshSurfaces()
    {
        isActionOnProgress = true;

        yield return new WaitForSeconds(delayInterval);
        for(int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }

        isActionOnProgress = false;
    }
}
