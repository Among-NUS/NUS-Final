using UnityEngine;
using UnityEngine.UI;

public class CooldownRingBehaviour : MonoBehaviour
{
    [Header("UI圆环")]
    public Image baseRing;              // 基础圆环背景（灰色）
    public Image currentRing;           // 当前可用基础圆环（蓝色）
    public Image usedRing;              // 正在使用的基础圆环（红色或黄色）

    [Header("指针设置")]
    public Transform pointer;           // 指针Transform
    public bool rotatePointer = true;   // 是否旋转指针
    public float pointerOffset = 0f;    // 指针角度偏移

    [Header("基础参数")]
    public int maxEnergy = 300;          // 最大基础值
    public int energyRegenRate = 1;      // 每秒恢复的基础
    public int energyConsumeRate = 1;    // 每秒消耗的基础

    private int currentEnergy = 300;     // 当前可用基础
    private int usedEnergy = 0;          // 正在使用的基础（录制时）
    private bool isRecording = false;    // 是否正在录制

    public bool CanStartRecording => currentEnergy > 0;
    public bool IsEnergyDepleted => currentEnergy <= 0;
    public int CurrentEnergy => currentEnergy;
    public int UsedEnergy => usedEnergy;

    void Start()
    {
        // 初始化基础为满
        currentEnergy = maxEnergy;
        usedEnergy = 0;

        // 确保圆环使用Radial 360模式
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
    /// 录制时每秒调用
    /// </summary>
    public void TickRecording()
    {
        if (!isRecording) return;

        // 增加使用的基础
        usedEnergy += energyConsumeRate;

        // 检查是否超过当前可用基础
        if (usedEnergy >= currentEnergy)
        {
            usedEnergy = currentEnergy;
            UpdateRingVisual();
            return; // 基础耗尽，应该停止录制
        }

        UpdateRingVisual();
    }

    /// <summary>
    /// 停止录制并消耗基础
    /// </summary>
    public void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;

        // 消耗实际使用的基础
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
        // 取消录制时不消耗基础，恢复到录制前的状态
        isRecording = false;
        usedEnergy = 0;
        // 如果需要，也可以恢复到满基础状态
        // currentEnergy = maxEnergy;
        UpdateRingVisual();
    }

    /// <summary>
    /// 每秒恢复基础（非录制时）
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
    /// 传送时消耗基础
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

                // 确保红色在蓝色之上
                usedRing.transform.SetAsLastSibling();
            }

            // 更新指针位置 - 指向剩余能量的末端
            UpdatePointer(availableRatio);
        }
        else
        {
            // 非录制时：只显示currentRing
            if (currentRing != null)
            {
                float currentRatio = Mathf.Clamp01((float)currentEnergy / maxEnergy);
                currentRing.fillAmount = currentRatio;

                // 更新指针位置 - 指向当前能量的末端
                UpdatePointer(currentRatio);
            }

            if (usedRing != null)
            {
                usedRing.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 更新指针位置
    /// </summary>
    private void UpdatePointer(float ratio)
    {
        if (pointer != null && rotatePointer)
        {
            // 计算角度：从顶部开始，顺时针旋转
            // Unity的Image.Origin360.Top从-90度开始，所以需要调整
            float angle = (ratio * 360f) - 90f + pointerOffset;
            pointer.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /// <summary>
    /// 检查是否基础耗尽
    /// </summary>
    public bool IsRecordingEnergyDepleted()
    {
        return isRecording && usedEnergy >= currentEnergy;
    }

    /// <summary>
    /// 手动设置指针角度（用于测试）
    /// </summary>
    public void SetPointerAngle(float angle)
    {
        if (pointer != null)
        {
            pointer.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /// <summary>
    /// 获取当前指针应该指向的角度
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