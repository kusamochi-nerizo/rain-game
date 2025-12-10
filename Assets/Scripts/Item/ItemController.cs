using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public float speed = 2f; // 移動速度

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveY = speed * Time.deltaTime;
        transform.Translate(new Vector3(0, moveY, 0));
    }
}