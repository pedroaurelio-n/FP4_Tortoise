using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> points;
    public bool canStart;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform parent;
    [SerializeField] private float timeDelay;
    [SerializeField] private float moveTime;
    [SerializeField] private Ease ease;
    [SerializeField] private Vector3 platformOffset;

    private int index;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        CheckStart();
    }

    public void CheckStart()
    {
        if (canStart)
        {
            points.Add(startPoint);

            foreach(Transform point in points)
            {
                point.parent = parent;
            }

            index = 0;

            StartCoroutine(Move(index));
        }
    }

    private IEnumerator Move(int index)
    {
        yield return new WaitForSeconds(timeDelay);

        rb.DOMove(points[index].position, moveTime).OnComplete(delegate { ChangePoint(); }).SetEase(ease);
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
            player.groundCheck.platformOffset = platformOffset;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out CharacterController player))
        {
            player.Move(rb.velocity * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            player.groundCheck.platformOffset = Vector3.zero;
            Debug.Log("exit");
        }
    }
}