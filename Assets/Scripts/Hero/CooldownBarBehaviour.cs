using UnityEngine;

public class CooldownBarBehaviour : MonoBehaviour
{
    [Header("UI���")]
    public RectTransform baseBar;        // ���������壨������
    public RectTransform currentBar;     // ��ǰ��������������ɫ��
    public RectTransform usedBar;        // ����ʹ�õ�����������ɫ���֣�

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
        UpdateBarVisual();
    }

    /// <summary>
    /// ��ʼ¼��
    /// </summary>
    public void StartRecording()
    {
        if (!CanStartRecording) return;

        isRecording = true;
        usedEnergy = 0;
        UpdateBarVisual();
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
            UpdateBarVisual();
            return; // �����ľ���Ӧ��ֹͣ¼��
        }

        UpdateBarVisual();
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
        UpdateBarVisual();
    }

    /// <summary>
    /// ȡ��¼��
    /// </summary>
    public void CancelRecording()
    {
        //只有在录制时把已用能量扣除
        if (isRecording)
        {
            currentEnergy -= usedEnergy;
            if (currentEnergy < 0) currentEnergy = 0;
        }
        isRecording = false;
        usedEnergy = 0;
        UpdateBarVisual();
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

        UpdateBarVisual();
    }

    /// <summary>
    /// ��Խʱ��������
    /// </summary>
    public void ConsumeEnergyForTravel()
    {
        currentEnergy -= energyConsumeRate;
        if (currentEnergy < 0) currentEnergy = 0;
        UpdateBarVisual();
    }

    /// <summary>
    /// ����UI��ʾ
    /// </summary>
    private void UpdateBarVisual()
    {
        if (isRecording)
        {
            // ¼��ʱ��currentBar��ʾʣ�࣬usedBar��ʾ����
            float availableEnergy = currentEnergy - usedEnergy;
            float usedRatio = Mathf.Clamp01((float)usedEnergy / maxEnergy);
            float availableRatio = Mathf.Clamp01((float)availableEnergy / maxEnergy);

            if (currentBar != null)
            {
                currentBar.localScale = new Vector3(availableRatio, 1f, 1f);
            }

            if (usedBar != null)
            {
                // ��usedBar��currentBar���Ҷ˿�ʼ
                usedBar.localScale = new Vector3(usedRatio, 1f, 1f);
                usedBar.anchoredPosition = new Vector2(availableRatio * baseBar.rect.width, 0);
                usedBar.gameObject.SetActive(true);
            }

            Debug.Log($"¼���� - ʣ������: {availableEnergy}/{currentEnergy}, ʹ��: {usedEnergy}, ʣ�����: {availableRatio}, ʹ�ñ���: {usedRatio}");
        }
        else
        {
            // ��¼��ʱ��ֻ��ʾcurrentBar
            if (currentBar != null)
            {
                float currentRatio = Mathf.Clamp01((float)currentEnergy / maxEnergy);
                currentBar.localScale = new Vector3(currentRatio, 1f, 1f);
            }

            if (usedBar != null)
            {
                usedBar.gameObject.SetActive(false);
            }

            Debug.Log($"��¼�� - ��ǰ����: {currentEnergy}/{maxEnergy}");
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