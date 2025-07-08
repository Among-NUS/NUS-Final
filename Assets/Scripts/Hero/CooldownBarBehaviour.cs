using UnityEngine;

public class CooldownBarBehaviour : MonoBehaviour
{
    [Header("UI���")]
    public RectTransform baseBar;        // 能量条本体（背景）
    public RectTransform currentBar;     // 当前可用能量条（蓝色）
    public RectTransform usedBar;        // 正在使用的能量条（红色遮罩）

    [Header("��������")]
    public float maxEnergy = 300f;          // 最大能量值
    public float energyRegenRate = 1f;      // 每帧恢复的能量
    public float energyConsumeRate = 1f;    // 每帧消耗的能量

    private float currentEnergy = 300f;     // 当前可用能量
    private float usedEnergy = 0f;          // 正在使用的能量（录制时）
    private bool isRecording = false;    // 是否正在录制

    public bool CanStartRecording => currentEnergy > 0;
    public bool IsEnergyDepleted => currentEnergy <= 0;
    public float CurrentEnergy => currentEnergy;
    public float UsedEnergy => usedEnergy;

    void Start()
    {
        // 初始化能量为满
        currentEnergy = maxEnergy;
        usedEnergy = 0;
        UpdateBarVisual();
    }

    /// <summary>
    /// 开始录制
    /// </summary>
    public void StartRecording()
    {
        if (!CanStartRecording) return;

        isRecording = true;
        usedEnergy = 0;
        UpdateBarVisual();
    }

    /// <summary>
    /// 录制时每帧调用
    /// </summary>
    public void TickRecording(float dt)
    {
        if (!isRecording) return;

        // 增加使用的能量
        usedEnergy += energyConsumeRate * dt;

        // 检查是否超过当前可用能量
        if (usedEnergy >= currentEnergy)
        {
            usedEnergy = currentEnergy;
            UpdateBarVisual();
            return; // 能量耗尽，应该停止录制
        }

        UpdateBarVisual();
    }

    /// <summary>
    /// ֹͣ停止录制并消耗能量
    /// </summary>
    public void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;

        // 消耗实际使用的能量
        currentEnergy -= usedEnergy;
        if (currentEnergy < 0) currentEnergy = 0;

        usedEnergy = 0;
        UpdateBarVisual();
    }

    /// <summary>
    /// 取消录制
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
    /// 每帧恢复能量（非录制时）
    /// </summary>
    public void RegenerateEnergy(float dt)
    {
        if (isRecording) return;

        currentEnergy += energyRegenRate * dt;
        if (currentEnergy > maxEnergy)
            currentEnergy = maxEnergy;

        UpdateBarVisual();
    }

    /// <summary>
    /// 穿越时消耗能量
    /// </summary>
    public void ConsumeEnergyForTravel(float dt)
    {
        currentEnergy -= energyConsumeRate * dt;
        if (currentEnergy < 0) currentEnergy = 0;
        UpdateBarVisual();
    }

    /// <summary>
    /// 更新UI显示ʾ
    /// </summary>
    private void UpdateBarVisual()
    {
        if (isRecording)
        {
            // 录制时：currentBar显示剩余，usedBar显示消耗
            float availableEnergy = currentEnergy - usedEnergy;
            float usedRatio = Mathf.Clamp01((float)usedEnergy / maxEnergy);
            float availableRatio = Mathf.Clamp01((float)availableEnergy / maxEnergy);

            if (currentBar != null)
            {
                currentBar.localScale = new Vector3(availableRatio, 1f, 1f);
            }

            if (usedBar != null)
            {
                // 让usedBar从currentBar的右端开始
                usedBar.localScale = new Vector3(usedRatio, 1f, 1f);
                usedBar.anchoredPosition = new Vector2(availableRatio * baseBar.rect.width, 0);
                usedBar.gameObject.SetActive(true);
            }

            Debug.Log($"录制中 - 剩余能量: {availableEnergy}/{currentEnergy}, 使用: {usedEnergy}, 剩余比例: {availableRatio}, 使用比例: {usedRatio}");
        }
        else
        {
            // 非录制时：只显示currentBar
            if (currentBar != null)
            {
                float currentRatio = Mathf.Clamp01((float)currentEnergy / maxEnergy);
                currentBar.localScale = new Vector3(currentRatio, 1f, 1f);
            }

            if (usedBar != null)
            {
                usedBar.gameObject.SetActive(false);
            }

            Debug.Log($"非录制 - 当前能量: {currentEnergy}/{maxEnergy}");
        }
    }

    /// <summary>
    /// 检查是否能量耗尽
    /// </summary>
    public bool IsRecordingEnergyDepleted()
    {
        return isRecording && usedEnergy >= currentEnergy;
    }
}