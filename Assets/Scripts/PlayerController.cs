using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject rainPrefab;
    [SerializeField] private int maxWaterStock = 10;
    CharacterController controller;
    Vector3 movedir = Vector3.zero;
    private float moveSpeed = 3.0f;
    private bool isGameOver = true;

    /// <summary>
    /// 水を発射できる回数
    /// </summary>
    [SerializeField] private int waterPower = 3;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        UpdateCloudScale();
        UpdateSpeed();
    }

    public void Init()
    {
        isGameOver = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || ContainsSpace(Input.inputString))
        {
            SpawnRain();
        }
    }

    bool ContainsSpace(string inputString)
    {
        // 入力文字列に半角スペースまたは全角スペースが含まれているか確認
        return inputString.Contains(" ") || inputString.Contains("　");
    }

    void FixedUpdate()
    {
        if (isGameOver)
        {
            return;
        }

        // movedir.z = Input.GetAxis("Vertical") * moveSpeed;
        movedir.x = Input.GetAxis("Horizontal") * moveSpeed;

        Vector3 globaldir = transform.TransformDirection(movedir);
        controller.Move(globaldir * Time.deltaTime);
    }

    void SpawnRain()
    {
        if (isGameOver)
        {
            return;
        }

        if (waterPower <= 0)
        {
            return;
        }

        Vector3 targetPosition = transform.position - new Vector3(0f, 1f, 0f);

        // Rain PrefabをInstantiateしてシーンに追加
        Instantiate(rainPrefab, targetPosition, Quaternion.identity);
        SubWaterPower(1);
        SoundManager.Instance.PlaySe("Water1");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver)
        {
            return;
        }

        if (other.CompareTag("Item"))
        {
            Destroy(other.gameObject); // 衝突した相手をDestroy
            AddWaterPower(1);
        }
    }

    void AddWaterPower(int value)
    {
        if (waterPower > maxWaterStock)
        {
            return;
        }

        waterPower += value;
        SoundManager.Instance.PlaySe("Water2");
        UpdateCloudScale();
        UpdateSpeed();
    }

    void SubWaterPower(int value)
    {
        waterPower -= value;
        UpdateCloudScale();
        UpdateSpeed();
    }

    void UpdateCloudScale()
    {
        // 現在のスケールを取得
        var o = this.gameObject;
        Vector3 currentScale = o.transform.localScale;

        var scale = 0.2f + (waterPower * 0.05f);

        // スケールの変更
        currentScale = new Vector3(scale, scale, scale);

        // 変更したスケールをGameObjectに適用
        o.transform.localScale = currentScale;
    }

    void UpdateSpeed()
    {
        moveSpeed = 4.0f - (waterPower * 0.3f);
    }

    public void Dispose()
    {
        isGameOver = true;
    }
}