using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    Transform playerTransform;
    Vector3 dif;

    void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        dif = transform.localPosition - playerTransform.localPosition;
    }

    void Update()
    {
        transform.localPosition = playerTransform.localPosition + dif;
    }
}
