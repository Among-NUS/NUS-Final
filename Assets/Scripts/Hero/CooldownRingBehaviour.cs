using UnityEngine;
using UnityEngine.UI;

public class CooldownRingBehaviour : MonoBehaviour
{
    [Header("UIԲ��")]
    public Image baseRing;              // ����Բ����������ɫ��
    public Image currentRing;           // ��ǰ���û���Բ������ɫ��
    public Image usedRing;              // ����ʹ�õĻ���Բ������ɫ���ɫ��

    [Header("ָ������")]
    public Transform pointer;           // ָ��Transform
    public bool rotatePointer = true;   // �Ƿ���תָ��
    public float pointerOffset = 0f;    // ָ��Ƕ�ƫ��

    [Header("��������")]
    public int maxEnergy = 300;          // ������ֵ
    public int energyRegenRate = 1;      // ÿ��ָ��Ļ���
    public int energyConsumeRate = 1;    // ÿ�����ĵĻ���

    private int currentEnergy = 300;     // ��ǰ���û���
    private int usedEnergy = 0;          // ����ʹ�õĻ�����¼��ʱ��
    private bool isRecording = false;    // �Ƿ�����¼��

    public bool CanStartRecording => currentEnergy > 0;
    public bool IsEnergyDepleted => currentEnergy <= 0;
    public int CurrentEnergy => currentEnergy;
    public int UsedEnergy => usedEnergy;

    void Start()
    {
        // ��ʼ������Ϊ��
        currentEnergy = maxEnergy;
        usedEnergy = 0;

        // ȷ��Բ��ʹ��Radial 360ģʽ
        SetupRingImages();
        UpdateRingVisual();
    }

    /// <summary>
    /// ����Բ��ͼ�����
    /// </summary>
    private void SetupRingImages()
    {
        if (baseRing != null)
        {
            baseRing.type = Image.Type.Filled;
            baseRing.fillMethod = Image.FillMethod.Radial360;
            baseRing.fillOrigin = (int)Image.Origin360.Top;
            baseRing.fillAmount = 1f;
        }

        if (currentRing != null)
        {
            currentRing.type = Image.Type.Filled;
            currentRing.fillMethod = Image.FillMethod.Radial360;
            currentRing.fillOrigin = (int)Image.Origin360.Top;
        }

        if (usedRing != null)
        {
            usedRing.type = Image.Type.Filled;
            usedRing.fillMethod = Image.FillMethod.Radial360;
            usedRing.fillOrigin = (int)Image.Origin360.Top;
        }
    }

    /// <summary>
    /// ��ʼ¼��
    /// </summary>
    public void StartRecording()
    {
        if (!CanStartRecording) return;

        isRecording = true;
        usedEnergy = 0;
        UpdateRingVisual();
    }

    /// <summary>
    /// ¼��ʱÿ�����
    /// </summary>
    public void TickRecording()
    {
        if (!isRecording) return;

        // ����ʹ�õĻ���
        usedEnergy += energyConsumeRate;

        // ����Ƿ񳬹���ǰ���û���
        if (usedEnergy >= currentEnergy)
        {
            usedEnergy = currentEnergy;
            UpdateRingVisual();
            return; // �����ľ���Ӧ��ֹͣ¼��
        }

        UpdateRingVisual();
    }

    /// <summary>
    /// ֹͣ¼�Ʋ����Ļ���
    /// </summary>
    public void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;

        // ����ʵ��ʹ�õĻ���
        currentEnergy -= usedEnergy;
        if (currentEnergy < 0) currentEnergy = 0;

        usedEnergy = 0;
        UpdateRingVisual();
    }

    /// <summary>
    /// ȡ��¼��
    /// </summary>
    public void CancelRecording()
    {
        // ȡ��¼��ʱ�����Ļ������ָ���¼��ǰ��״̬
        isRecording = false;
        usedEnergy = 0;
        // �����Ҫ��Ҳ���Իָ���������״̬
        // currentEnergy = maxEnergy;
        UpdateRingVisual();
    }

    /// <summary>
    /// ÿ��ָ���������¼��ʱ��
    /// </summary>
    public void RegenerateEnergy()
    {
        if (isRecording) return;

        currentEnergy += energyRegenRate;
        if (currentEnergy > maxEnergy)
            currentEnergy = maxEnergy;

        UpdateRingVisual();
    }

    /// <summary>
    /// ����ʱ���Ļ���
    /// </summary>
    public void ConsumeEnergyForTravel()
    {
        currentEnergy -= energyConsumeRate;
        if (currentEnergy < 0) currentEnergy = 0;
        UpdateRingVisual();
    }

    /// <summary>
    /// ����UI��ʾ
    /// </summary>
    private void UpdateRingVisual()
    {
        if (isRecording)
        {
            // ¼��ʱ��currentRing��ʾʣ�࣬usedRing��ʾ����
            float availableEnergy = currentEnergy - usedEnergy;
            float usedRatio = Mathf.Clamp01((float)usedEnergy / maxEnergy);
            float availableRatio = Mathf.Clamp01((float)availableEnergy / maxEnergy);

            if (currentRing != null)
            {
                currentRing.fillAmount = availableRatio;
            }

            if (usedRing != null)
            {
                usedRing.fillAmount = usedRatio;
                usedRing.gameObject.SetActive(true);

                // ȷ����ɫ����ɫ֮��
                usedRing.transform.SetAsLastSibling();
            }

            // ����ָ��λ�� - ָ��ʣ��������ĩ��
            UpdatePointer(availableRatio);
        }
        else
        {
            // ��¼��ʱ��ֻ��ʾcurrentRing
            if (currentRing != null)
            {
                float currentRatio = Mathf.Clamp01((float)currentEnergy / maxEnergy);
                currentRing.fillAmount = currentRatio;

                // ����ָ��λ�� - ָ��ǰ������ĩ��
                UpdatePointer(currentRatio);
            }

            if (usedRing != null)
            {
                usedRing.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ����ָ��λ��
    /// </summary>
    private void UpdatePointer(float ratio)
    {
        if (pointer != null && rotatePointer)
        {
            // ����Ƕȣ��Ӷ�����ʼ��˳ʱ����ת
            // Unity��Image.Origin360.Top��-90�ȿ�ʼ��������Ҫ����
            float angle = (ratio * 360f) - 90f + pointerOffset;
            pointer.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /// <summary>
    /// ����Ƿ�����ľ�
    /// </summary>
    public bool IsRecordingEnergyDepleted()
    {
        return isRecording && usedEnergy >= currentEnergy;
    }

    /// <summary>
    /// �ֶ�����ָ��Ƕȣ����ڲ��ԣ�
    /// </summary>
    public void SetPointerAngle(float angle)
    {
        if (pointer != null)
        {
            pointer.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /// <summary>
    /// ��ȡ��ǰָ��Ӧ��ָ��ĽǶ�
    /// </summary>
    public float GetCurrentPointerAngle()
    {
        float ratio;
        if (isRecording)
        {
            float availableEnergy = currentEnergy - usedEnergy;
            ratio = Mathf.Clamp01((float)availableEnergy / maxEnergy);
        }
        else
        {
            ratio = Mathf.Clamp01((float)currentEnergy / maxEnergy);
        }

        return (ratio * 360f) - 90f + pointerOffset;
    }
}