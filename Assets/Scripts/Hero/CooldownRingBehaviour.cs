﻿using UnityEngine;
using UnityEngine.UI;

public class CooldownRingBehaviour : MonoBehaviour
{
    /* ──────────── UI & 参数 ──────────── */
    [Header("UI圆环")]
    public Image baseRing;
    public Image currentRing;
    public Image usedRing;

    [Header("指针设置")]
    public Transform pointer;
    public bool rotatePointer = true;
    public float pointerOffset = 0f;

    [Header("基础参数")]
    public int maxEnergy = 300;
    public int energyRegenRate = 1;     // 每 FixedUpdate 恢复
    public int energyConsumeRate = 1;   // 每 FixedUpdate 消耗

    int currentEnergy = 300;            // 剩余可用能量
    int usedEnergy = 0;                 // 本次录制已用能量

    /* ──────────── 便捷属性 ──────────── */
    bool IsRecording =>
        GameManager.Instance != null &&
        GameManager.Instance.currentPhase == GameManager.GamePhase.Recording;

    bool IsTimeStop =>
        GameManager.Instance != null &&
        GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop;

    public bool CanStartRecording => currentEnergy > 0;
    public bool IsEnergyDepleted => currentEnergy <= 0;
    public bool IsRecordingEnergyDepleted() =>
        IsRecording && usedEnergy >= currentEnergy;

    public int CurrentEnergy => currentEnergy;
    public int UsedEnergy => usedEnergy;

    /* ──────────── 生命周期 ──────────── */
    void Start()
    {
        currentEnergy = maxEnergy;
        SetupRingImages();
        UpdateRingVisual();
    }

    /* ──────────── 每帧能量驱动 ──────────── */
    void FixedUpdate()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        switch (gm.currentPhase)
        {
            case GameManager.GamePhase.TimeStop:
                ConsumeEnergyForTravel();
                if (IsEnergyDepleted)
                    gm.BeginReplay();                      // 自动结束时间静止
                break;

            case GameManager.GamePhase.Recording:
                TickRecording();
                if (IsRecordingEnergyDepleted())
                    gm.CancelRecording();                  // 能量不足，终止录制
                break;

            case GameManager.GamePhase.Normal:
                RegenerateEnergy();
                break;
        }
    }

    /* ──────────── 录制阶段接口 ──────────── */
    public void StartRecording()
    {
        if (!CanStartRecording) return;

        usedEnergy = 0;
        UpdateRingVisual();
    }

    public void TickRecording()
    {
        if (!IsRecording) return;

        usedEnergy += energyConsumeRate;
        usedEnergy = Mathf.Min(usedEnergy, currentEnergy);

        UpdateRingVisual();
    }

    public void StopRecording()            // 录制正常结束
    {
        if (!IsRecording && usedEnergy == 0) return;

        currentEnergy = Mathf.Max(currentEnergy - usedEnergy, 0);
        usedEnergy = 0;

        UpdateRingVisual();
    }

    public void CancelRecording()          // GM 外部调用
    {
        usedEnergy = 0;
        UpdateRingVisual();
    }

    /* ──────────── 能量增减 ──────────── */
    public void RegenerateEnergy()
    {
        if (IsRecording || IsTimeStop) return;

        currentEnergy = Mathf.Min(currentEnergy + energyRegenRate, maxEnergy);
        UpdateRingVisual();
    }

    public void ConsumeEnergyForTravel()
    {
        currentEnergy = Mathf.Max(currentEnergy - energyConsumeRate, 0);
        UpdateRingVisual();
    }

    /* ──────────── UI 绘制 ──────────── */
    void SetupRingImages()
    {
        Image[] rings = { baseRing, currentRing, usedRing };
        foreach (var img in rings)
        {
            if (!img) continue;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Radial360;
            img.fillOrigin = (int)Image.Origin360.Top;
        }
        if (baseRing) baseRing.fillAmount = 1f;
    }

    void UpdateRingVisual()
    {
        if (IsRecording)
        {
            float remaining = currentEnergy - usedEnergy;
            currentRing.fillAmount = Mathf.Clamp01(remaining / (float)maxEnergy);

            usedRing.gameObject.SetActive(true);
            usedRing.fillAmount = Mathf.Clamp01(usedEnergy / (float)maxEnergy);
            usedRing.transform.SetAsLastSibling();

            UpdatePointer(currentRing.fillAmount);
        }
        else
        {
            currentRing.fillAmount = Mathf.Clamp01(currentEnergy / (float)maxEnergy);
            UpdatePointer(currentRing.fillAmount);
            usedRing.gameObject.SetActive(false);
        }

        // 层级：base < used < current < pointer
        baseRing.transform.SetSiblingIndex(0);
        usedRing.transform.SetSiblingIndex(1);
        currentRing.transform.SetSiblingIndex(2);
        if (pointer) pointer.transform.SetSiblingIndex(3);
    }

    void UpdatePointer(float ratio)
    {
        if (pointer && rotatePointer)
        {
            float angle = -(ratio * 360f) + pointerOffset;
            pointer.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
