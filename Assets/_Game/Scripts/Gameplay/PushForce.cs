using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Vector3Direction
{
    Forward,
    Backward,
    Right,
    Left,
    Up,
    Down,
    RadialOut,
    RadialIn
}

public class PushForce : MonoBehaviour
{
    [SerializeField] private Vector3Direction pushDirection;
    [SerializeField] private float pushForce;
    [SerializeField] private float startDuration;
    [SerializeField] private float endDuration;

    private Vector3 _pushDirection;

    private PlayerMovement playerMovement;
    
    private void Start()
    {
        UpdatePushForceDirection();
    }

    private void Update()
    {
        UpdatePushForceDirection();
    }

    public void SetPlayer(PlayerMovement player)
    {
        playerMovement = player;
    }

    public void UpdatePushForceDirection()
    {
        switch (pushDirection)
        {
            case Vector3Direction.Forward: _pushDirection = transform.forward; break;
            case Vector3Direction.Backward: _pushDirection = -transform.forward; break;
            case Vector3Direction.Right: _pushDirection = transform.right; break;
            case Vector3Direction.Left: _pushDirection = -transform.right; break;
            case Vector3Direction.Up: _pushDirection = transform.up; break;
            case Vector3Direction.Down: _pushDirection = -transform.up; break;
            case Vector3Direction.RadialOut:
                if (playerMovement == null)
                    return;
                
                _pushDirection = new Vector3
                (
                    (playerMovement.transform.position - transform.position).x,
                    0f,
                    (playerMovement.transform.position - transform.position).z
                );
                break;
            case Vector3Direction.RadialIn:
                if (playerMovement == null)
                    return;

                _pushDirection = new Vector3
                (
                    (transform.position - playerMovement.transform.position).x,
                    0f,
                    (transform.position - playerMovement.transform.position).z
                );
                break;
        }
    }

    public void TriggerPush()
    {
        playerMovement.SetPushForceAreaBool(true);
            
        if (pushDirection == Vector3Direction.Up)
            playerMovement.SetPushForceUpBool(true);
        else
            playerMovement.SetPushForceUpBool(false);

        playerMovement.ActivatePushForce(_pushDirection, pushForce, startDuration);
        Debug.Log($"test");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            if (playerMovement == null)
                playerMovement = player;


            TriggerPush();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            playerMovement.SetPushForceAreaBool(false);
            playerMovement.DeactivatePushForce(endDuration);
        }
    }
}
