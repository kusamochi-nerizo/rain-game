using UnityEngine;

public class ItemDestroyer : MonoBehaviour
{
    void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Destroy(other.gameObject); // 衝突した相手をDestroy
        }
    }
}