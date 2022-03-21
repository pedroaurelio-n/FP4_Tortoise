using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> points;
    public bool canStart;
    [SerializeField] private Transform _Dynamic;
    [SerializeField] private float timeDelay;
    [SerializeField] private float moveTime;
    [SerializeField] private bool isSequential = true;
    [SerializeField] private Ease ease;

    private int index;
    private bool isMoving;
    private bool isPlayerOnPlatform;
    private CharacterController playerC;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool GetSequential()
    {
        return isSequential;
    }

    private void Start()
    {
        foreach(Transform point in points)
        {
            if (_Dynamic != null)
                point.parent = _Dynamic;
            else
                point.parent = null;
        }

        CheckStart();
    }

    private void FixedUpdate()
    {
        if (playerC == null)
            return;

        if (isPlayerOnPlatform)
        {
            Debug.Log(rb.velocity * Time.deltaTime);
            playerC.Move(rb.velocity * Time.deltaTime);
        }
            
    }

    public void CheckStart()
    {
        if (canStart)
        {
            index = 1;
            
            StartCoroutine(Move(index));
        }
    }

    private IEnumerator Move(int index)
    {
        yield return new WaitForSeconds(timeDelay);


        if (isSequential)
        {
            rb.DOMove(points[index].position, moveTime).OnComplete(delegate { ChangePoint(); }).SetEase(ease).SetUpdate(UpdateType.Fixed);
        }
        else
        {
            isMoving = true;
            rb.DOMove(points[index].position, moveTime).OnComplete(delegate { isMoving = false; }).SetEase(ease).SetUpdate(UpdateType.Fixed);
        }
    }

    public void GoToNextPoint()
    {
        if (!isMoving)
            ChangePoint();
    }

    private void ChangePoint()
    {
        index++;

        if (index >= points.Count)
            index = 0;

        StartCoroutine(Move(index));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            Debug.Log("enter");
            player.groundCheck.SetMovingPlatformBool(true);

            isPlayerOnPlatform = true;

            if (playerC == null)
            {
                if (player.TryGetComponent(out CharacterController controller))
                {
                    playerC = controller;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            Debug.Log("exit");
            player.groundCheck.SetMovingPlatformBool(false);
            
            isPlayerOnPlatform = false;
        }
    }
}