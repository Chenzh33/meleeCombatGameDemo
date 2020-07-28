using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform playerTransform;
    private Vector3 targetPos;
    public float moveSpeed;
    private Vector3 offset;

    void Start () {
        offset = transform.position - playerTransform.position;

    }

    void Update () {
        targetPos = playerTransform.position + offset;
        Vector3 velocity = (targetPos - transform.position) * moveSpeed;
        transform.position = Vector3.SmoothDamp (transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);
    }
}