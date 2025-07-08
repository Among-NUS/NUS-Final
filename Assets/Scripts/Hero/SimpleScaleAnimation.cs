using UnityEngine;
using System.Collections;

public class SimpleScaleAnimation : MonoBehaviour
{
    [Header("动画设置")]
    public float animationSpeed = 2f;
    public float disappearTime = 0.5f;

    private Vector3 originalScale;
    private bool isAnimating = false;

    void Start()
    {
        originalScale = transform.localScale;
        Debug.Log("原始大小: " + originalScale);
    }

    public void PlayAnimation()
    {
        Debug.Log("PlayAnimation被调用");
        if (!isAnimating)
        {
            Debug.Log("开始动画");
            StartCoroutine(ScaleAnimation());
        }
        else
        {
            Debug.Log("动画正在进行中，忽略调用");
        }
    }

    IEnumerator ScaleAnimation()
    {
        isAnimating = true;
        Debug.Log("协程开始");

        // 缩小到0
        float timer = 0f;
        while (transform.localScale.x > 0.01f)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, animationSpeed * Time.deltaTime);
            yield return null;

            // 防止无限循环
            if (timer > 5f)
            {
                Debug.Log("缩小超时，强制设为0");
                break;
            }
        }
        transform.localScale = Vector3.zero;
        Debug.Log("缩小完成");

        // 等待
        yield return new WaitForSeconds(disappearTime);
        Debug.Log("等待完成，开始放大");

        // 放大到原始大小
        timer = 0f;
        while (transform.localScale.x < originalScale.x * 0.95f)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, animationSpeed * Time.deltaTime);
            yield return null;

            // 防止无限循环
            if (timer > 5f)
            {
                Debug.Log("放大超时，强制设为原始大小");
                break;
            }
        }
        transform.localScale = originalScale;
        Debug.Log("放大完成，动画结束");

        isAnimating = false;
    }

    // 测试方法，按T键测试
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("按下T键，测试动画");
            PlayAnimation();
        }
    }
}