using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DisposeUtil
{
    public static void  DestroyChildrenObjects(GameObject parent)
    {
        // 親オブジェクトの子供の数を取得
        int childCount = parent.transform.childCount;

        // 子供を逐次削除
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = parent.transform.GetChild(i);
            Object.Destroy(child.gameObject);
        }
    }
}