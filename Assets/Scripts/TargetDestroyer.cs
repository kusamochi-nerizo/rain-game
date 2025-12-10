using System;
using UnityEngine;

public class TargetDestroyer : MonoBehaviour
{
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            Destroy(other.gameObject); // 衝突した相手をDestroy
        }
    }
}