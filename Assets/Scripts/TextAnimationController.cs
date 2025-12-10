using System;
using UnityEngine;

public class TextAnimationController : MonoBehaviour
{
    private Animator animator;
    private Action onComplete;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void PlayAnimation(Action onCompleteAction)
    {
        animator.enabled = true;
        onComplete = onCompleteAction;
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
    }

    public void OnAnimationEnd()
    {
        animator.enabled = false;
        onComplete?.Invoke();
    }
}