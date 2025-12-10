using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private float speed = 1f; // 移動速度
    [SerializeField] private GameObject flowerObject;
    [SerializeField] private GameObject budObject; // つぼみ
    private bool isDead = false;

    void Start()
    {
    }

    public void Init(float speedMin, float speedMax)
    {
        speed = MathUtil.GetRandomValue(speedMin, speedMax);
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        // X軸方向に移動
        float moveX = speed * Time.deltaTime;
        transform.Translate(new Vector3(moveX, 0, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
        {
            return;
        }

        // 衝突した相手がrainPrefabであれば、両方のPrefabをDestroyする
        if (other.gameObject.CompareTag($"Rain"))
        {
            Destroy(other.gameObject); // 衝突した相手をDestroy
            ScoreManager.Instance.IncreaseScore(1);
            OnDestroyAction();
        }
    }

    void OnDestroyAction()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        // DisableCollider();
        budObject.SetActive(false);
        flowerObject.SetActive(true);
        SoundManager.Instance.PlaySe("Flower");
        // await Task.Delay(1000).ConfigureAwait(false);
        Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(_ => Destroy(this.gameObject))
            .AddTo(this);
    }
}