using System;
using UniRx;
using UnityEngine;

public class CatController : MonoBehaviour
{
    private Animator animator;
    private bool isDead = false;
    private float speed = 1f; // 移動速度
    private float oldSpeed = 1.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void Init(float speedMin, float speedMax)
    {
        speed = MathUtil.GetRandomValue(speedMin, speedMax);
    }

    private void FixedUpdate()
    {
        // TODO 暫定対応
        float moveZ = Math.Abs(speed) * Time.deltaTime;
        var moveDirection = new Vector3(0, 0, moveZ);
        RotateAvatar();
        transform.Translate(moveDirection);
    }

    void RotateAvatar()
    {
        // TODO 暫定対応
        if (speed > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (speed < 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
        {
            return;
        }

        if (other.gameObject.CompareTag($"Rain"))
        {
            Destroy(other.gameObject); // 衝突した相手をDestroy
            ScoreManager.Instance.IncreaseScore(-1);
            oldSpeed = speed;
            speed = 0;
            isDead = true;
            SoundManager.Instance.PlaySe("Cat");
            animator.SetTrigger("hit");
            Observable.Timer(TimeSpan.FromSeconds(0.9))
                .Subscribe(_ => DestroyAction())
                .AddTo(this);
        }
    }

    private void DestroyAction()
    {
        if (oldSpeed > 0)
        {
            speed = 3;
        }
        else
        {
            speed = -3;
        }

        animator.SetTrigger("run");
    }
}