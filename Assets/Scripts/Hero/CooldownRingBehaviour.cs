using UnityEngine;
using UnityEngine.UI;

public class CooldownRingBehaviour : MonoBehaviour
{
    [Header("UI圆环")]
    public Image baseRing;              // 能量圆环本体（背景）
    public Image currentRing;           // 当前可用能量圆环（蓝色）
    public Image usedRing;              // 正在使用的能量圆环（红色遮罩）

    [Header("能量参数")]
    public int maxEnergy = 300;          // 最大能量值
    public int energyRegenRate = 1;      // 每帧恢复的能量
    public int energyConsumeRate = 1;    // 每帧消耗的能量

    private int currentEnergy = 300;     // 当前可用能量
    private int usedEnergy = 0;          // 正在使用的能量（录制时）
    private bool isRecording = false;    // 是否正在录制

    public bool CanStartRecording => currentEnergy > 0;
    public bool IsEnergyDepleted => currentEnergy <= 0;
    public int CurrentEnergy => currentEnergy;
    public int UsedEnergy => usedEnergy;

    void Start()
    {
        // 初始化能量为满
        currentEnergy = maxEnergy;
        usedEnergy = 0;

        // 确保圆环使用Radial 360填充模式
        SetupRingImages();
        UpdateRingVisual();
    }

    /// <summary>
    /// 设置圆环图像参数
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
    /// 开始录制
    /// </summary>
    public void StartRecording()
    {
        if (!CanStartRecording) return;

        isRecording = true;
        usedEnergy = 0;
        UpdateRingVisual();
    }

    /// <summary>
    /// 录制时每帧调用
    /// </summary>
    public void TickRecording()
    {
        if (!isRecording) return;

        // 增加使用的能量
        usedEnergy += energyConsumeRate;

        // 检查是否超过当前可用能量
        if (usedEnergy >= currentEnergy)
        {
            usedEnergy = currentEnergy;
            UpdateRingVisual();
            return; // 能量耗尽，应该停止录制
        }

        UpdateRingVisual();
    }

    /// <summary>
    /// 停止录制并消耗能量
    /// </summary>
    public void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;

        // 消耗实际使用的能量
        currentEnergy -= usedEnergy;
        if (currentEnergy < 0) currentEnergy = 0;

        usedEnergy = 0;
        UpdateRingVisual();
    }

    /// <summary>
    /// 取消录制
    /// </summary>
    public void CancelRecording()
    {
        // 取消录制时不消耗能量，恢复到录制前的状态
        isRecording = false;
        usedEnergy = 0;
        // 如果需要，也可以恢复到满能量状态
        // currentEnergy = maxEnergy;
        UpdateRingVisual();
    }

    /// <summary>
    /// 每帧恢复能量（非录制时）
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
    /// 穿越时消耗能量
    /// </summary>
    public void ConsumeEnergyForTravel()
    {
        currentEnergy -= energyConsumeRate;
        if (currentEnergy < 0) currentEnergy = 0;
        UpdateRingVisual();
    }

    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateRingVisual()
    {
        if (isRecording)
        {
            // 录制时：currentRing显示剩余，usedRing显示消耗
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

                // 确保红圈在蓝圈之上
                usedRing.transform.SetAsLastSibling();
            }
        }
        else
        {
            // 非录制时：只显示currentRing
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
    /// 检查是否能量耗尽
    /// </summary>
    public bool IsRecordingEnergyDepleted()
    {
        return isRecording && usedEnergy >= currentEnergy;
    }
}