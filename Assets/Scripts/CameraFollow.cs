using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    Transform playerTransform;
	private Vector3 targetPos;
    public float moveSpeed;

    void Start () {

    }

    void Update () {
        if (playerTransform != null) {
            targetPos = new Vector3 (playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
            Vector3 velocity = (targetPos - transform.position) * moveSpeed;
            transform.position = Vector3.SmoothDamp (transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);
        }
    }
}