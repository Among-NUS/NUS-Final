using System.Collections;
using UnityEngine;

public class MessageDisplay : MonoBehaviour
{
    public GameObject messagePanel; // ✅ 只隐藏这个
    public float duration = 1f;

    public void ShowMessageForOneSecond()
    {
        StopAllCoroutines(); // 避免冲突
        StartCoroutine(ShowAndHide());
    }

    IEnumerator ShowAndHide()
    {
        messagePanel.SetActive(true);         // ✅ 显示文本内容
        yield return new WaitForSeconds(1f);  // ⏱️ 等待一秒
        messagePanel.SetActive(false);        // ✅ 隐藏
    }
}
