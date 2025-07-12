using UnityEngine;
using UnityEngine.UI;

public class CooldownRingBehaviour : MonoBehaviour
{
    [Header("UIԲ��")]
    public Image baseRing;              // ����Բ�����壨������
    public Image currentRing;           // ��ǰ��������Բ������ɫ��
    public Image usedRing;              // ����ʹ�õ�����Բ������ɫ���֣�

    [Header("��������")]
    public int maxEnergy = 300;          // �������ֵ
    public int energyRegenRate = 1;      // ÿ֡�ָ�������
    public int energyConsumeRate = 1;    // ÿ֡���ĵ�����

    private int currentEnergy = 300;     // ��ǰ��������
    private int usedEnergy = 0;          // ����ʹ�õ�������¼��ʱ��
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

        // ȷ��Բ��ʹ��Radial 360���ģʽ
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
    /// ¼��ʱÿ֡����
    /// </summary>
    public void TickRecording()
    {
        if (!isRecording) return;

        // ����ʹ�õ�����
        usedEnergy += energyConsumeRate;

        // ����Ƿ񳬹���ǰ��������
        if (usedEnergy >= currentEnergy)
        {
            usedEnergy = currentEnergy;
            UpdateRingVisual();
            return; // �����ľ���Ӧ��ֹͣ¼��
        }

        UpdateRingVisual();
    }

    /// <summary>
    /// ֹͣ¼�Ʋ���������
    /// </summary>
    public void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;

        // ����ʵ��ʹ�õ�����
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
        // ȡ��¼��ʱ�������������ָ���¼��ǰ��״̬
        isRecording = false;
        usedEnergy = 0;
        // �����Ҫ��Ҳ���Իָ���������״̬
        // currentEnergy = maxEnergy;
        UpdateRingVisual();
    }

    /// <summary>
    /// ÿ֡�ָ���������¼��ʱ��
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
    /// ��Խʱ��������
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

                // ȷ����Ȧ����Ȧ֮��
                usedRing.transform.SetAsLastSibling();
            }
        }
        else
        {
            // ��¼��ʱ��ֻ��ʾcurrentRing
            if (currentRing != null)
            {
                float currentRatio = Mathf.Clamp01((float)currentEnergy / maxEnergy);
                currentRing.fillAmount = currentRatio;
            }

            if (usedRing != null)
            {
                usedRing.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ����Ƿ������ľ�
    /// </summary>
    public bool IsRecordingEnergyDepleted()
    {
        return isRecording && usedEnergy >= currentEnergy;
    }
}