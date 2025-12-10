using UnityEngine;

public class TargetFactory : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject targetPrefab2;
    [SerializeField] private GameObject parent;
    [SerializeField] private float speedMin = 0.5f;
    [SerializeField] private float speedMax = 2.0f;
    [SerializeField] private float spawnDuration = 1.0f;

    private float time = 0f;
    private bool isGameOver = false;

    void Start()
    {
    }

    public void Init()
    {
        isGameOver = false;
    }

    // Update is called once per frame
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
        float randomValue = Random.value;

        if (randomValue <= 0.8f)
        {
            var obj = Instantiate(targetPrefab, transform.position, Quaternion.identity, parent.transform);
            var controller = obj.GetComponent<TargetController>();
            controller.Init(speedMin, speedMax);
        }
        else
        {
            var pos = transform.position;
            pos.y = 0;
            var obj1 = Instantiate(targetPrefab2, pos, Quaternion.identity, parent.transform);
            var controller1 = obj1.GetComponent<CatController>();
            controller1.Init(speedMin, speedMax);
        }
    }

    public void Dispose()
    {
        isGameOver = true;
        DisposeUtil.DestroyChildrenObjects(parent);
    }
}