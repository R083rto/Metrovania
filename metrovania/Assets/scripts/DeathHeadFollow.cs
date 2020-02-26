using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHeadFollow : MonoBehaviour
{
    [SerializeField] private float headMoveAmount = 2;
    [SerializeField] private Transform target;
    private Vector3 startHeadPos;


    // Start is called before the first frame update
    void Start()
    {
        startHeadPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.position - transform.position;
        if(dir.sqrMagnitude >1)
        transform.localPosition = Vector3.Lerp(transform.localPosition, startHeadPos + dir.normalized * 0.5f, Time.deltaTime * headMoveAmount);
    }
}
