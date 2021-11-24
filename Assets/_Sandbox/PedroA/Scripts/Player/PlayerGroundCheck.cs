using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    public bool isGrounded;

    [Header("Grounded Configs")]
    public Vector3 platformOffset; 
    [SerializeField] private Vector3 offset;
    [SerializeField] private float startRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float activationDelay;

    private float _radius;
    private bool isOnMovingPlatform;

    private void Start()
    {
        isGrounded = true;
        _radius = startRadius;
    }

    private void GroundCheck(bool check)
    {
        if (isGrounded != check)
            isGrounded = check;
    }

    private void LateUpdate()
    {
        var colliders = Physics.OverlapSphere(transform.position + offset + platformOffset, _radius, groundLayer);
        bool isColliding = colliders.Length != 0;

        GroundCheck(isColliding);
    }

    public void ActivateGroundCheck() { _radius = startRadius; }
    public void DeactivateGroundCheck() { _radius = 0; }

    public void SetMovingPlatformBool(bool value) { isOnMovingPlatform = value; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + offset + platformOffset, _radius);
    }
}
