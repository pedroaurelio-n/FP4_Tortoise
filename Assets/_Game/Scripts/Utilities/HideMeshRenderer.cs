using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMeshRenderer : MonoBehaviour
{
    private MeshRenderer mesh;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        mesh.enabled = false;
    }
}
