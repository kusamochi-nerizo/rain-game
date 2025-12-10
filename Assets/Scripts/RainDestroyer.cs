using System;
using UnityEngine;

public class RainDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag($"Rain"))
        {
            Destroy(other.gameObject); // 衝突した相手をDestroy
        }
    }
}