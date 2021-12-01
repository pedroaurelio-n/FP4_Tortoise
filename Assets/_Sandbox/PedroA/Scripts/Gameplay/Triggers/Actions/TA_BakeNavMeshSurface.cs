using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TA_BakeNavMeshSurface : TriggerAction
{
    [SerializeField] private NavMeshSurface[] surfaces;

    public override bool TryToActivateAction()
    {
        if (CanActivateAction())
            ActivateAction();
        else
            Debug.Log("Couldn't perform action");

        return CanActivateAction();
    }

    protected override bool CanActivateAction()
    {
        return true;
    }

    protected override void ActivateAction()
    {
        StartCoroutine(BakeNavMeshSurfaces());
    }

    private IEnumerator BakeNavMeshSurfaces()
    {
        isActionOnProgress = true;

        yield return new WaitForSeconds(delayToActivate);
        for(int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }

        isActionOnProgress = false;
    }
}
