using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> points;
    [SerializeField] private List<bool> activateKinematic;
    public bool canStart;
    [SerializeField] private Transform _Dynamic;
    [SerializeField] private float timeDelay;
    [SerializeField] private float moveTime;
    [SerializeField] private bool isSequential = true;
    [SerializeField] private Ease ease;
    [SerializeField] private Vector3 platformOffset;

    private int index;
    private bool isMoving;

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
            rb.DOMove(points[index].position, moveTime).OnComplete(delegate { ChangePoint(); }).SetEase(ease);
        }
        else
        {
            isMoving = true;
            rb.DOMove(points[index].position, moveTime).OnComplete(delegate { isMoving = false; }).SetEase(ease);
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
        
        rb.isKinematic = activateKinematic[index];

        StartCoroutine(Move(index));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            Debug.Log("enter");
            player.groundCheck.SetMovingPlatformBool(true);
            player.groundCheck.platformOffset = platformOffset;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out CharacterController playerController))
        {
            Debug.Log("stay");
            Debug.Log(rb.velocity * Time.deltaTime);
            playerController.Move(rb.velocity * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            Debug.Log("exit");
            player.groundCheck.SetMovingPlatformBool(false);
            player.groundCheck.platformOffset = Vector3.zero;
        }
    }
}