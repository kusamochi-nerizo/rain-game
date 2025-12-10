using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject parent;
    [SerializeField] private float spawnDuration = 0.7f;
    private float time = 0f;
    private Bounds objectBounds;
    private bool isGameOver = false;

    void Start()
    {
        objectBounds = GetComponent<Renderer>().bounds;
    }

    public void Init()
    {
        isGameOver = false;
    }

    void FixedUpdate()
    {
        if (isGameOver)
        {
            return;
        }

        time += Time.deltaTime;
        if (time > spawnDuration)
        {
            time = 0;
            SpawnTarget();
        }
    }

    void SpawnTarget()
    {
        // ランダムな座標を取得
        Vector3 randomPosition = GetRandomPositionWithinBounds(objectBounds);
        Instantiate(targetPrefab, randomPosition, Quaternion.identity, parent.transform);
    }

    Vector3 GetRandomPositionWithinBounds(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);

        var position = transform.position;
        return new Vector3(randomX, position.y, position.z);
    }

    public void Dispose()
    {
        isGameOver = true;
        DisposeUtil.DestroyChildrenObjects(parent);
    }
}