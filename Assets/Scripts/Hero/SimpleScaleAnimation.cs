using UnityEngine;
using System.Collections;

public class SimpleScaleAnimation : MonoBehaviour
{
    [Header("��������")]
    public float animationSpeed = 2f;
    public float disappearTime = 0.5f;

    private Vector3 originalScale;
    private bool isAnimating = false;

    void Start()
    {
        originalScale = transform.localScale;
        Debug.Log("ԭʼ��С: " + originalScale);
    }

    public void PlayAnimation()
    {
        Debug.Log("PlayAnimation������");
        if (!isAnimating)
        {
            Debug.Log("��ʼ����");
            StartCoroutine(ScaleAnimation());
        }
        else
        {
            Debug.Log("�������ڽ����У����Ե���");
        }
    }

    IEnumerator ScaleAnimation()
    {
        isAnimating = true;
        Debug.Log("Э�̿�ʼ");

        // ��С��0
        float timer = 0f;
        while (transform.localScale.x > 0.01f)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, animationSpeed * Time.deltaTime);
            yield return null;

            // ��ֹ����ѭ��
            if (timer > 5f)
            {
                Debug.Log("��С��ʱ��ǿ����Ϊ0");
                break;
            }
        }
        transform.localScale = Vector3.zero;
        Debug.Log("��С���");

        // �ȴ�
        yield return new WaitForSeconds(disappearTime);
        Debug.Log("�ȴ���ɣ���ʼ�Ŵ�");

        // �Ŵ�ԭʼ��С
        timer = 0f;
        while (transform.localScale.x < originalScale.x * 0.95f)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, animationSpeed * Time.deltaTime);
            yield return null;

            // ��ֹ����ѭ��
            if (timer > 5f)
            {
                Debug.Log("�Ŵ�ʱ��ǿ����Ϊԭʼ��С");
                break;
            }
        }
        transform.localScale = originalScale;
        Debug.Log("�Ŵ���ɣ���������");

        isAnimating = false;
    }

    // ���Է�������T������
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("����T�������Զ���");
            PlayAnimation();
        }
    }
}