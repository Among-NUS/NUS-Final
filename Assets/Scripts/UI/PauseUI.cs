﻿using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;  // ✅ 需要控制按钮交互

public class PauseUI : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject confirmPanel;
    public GameObject settingsPanel;
    public GameObject inGameUICanvas;
    public TextMeshProUGUI confirmText;

    private System.Action confirmAction;

    void Start()
    {
        pausePanel.SetActive(false);
        confirmPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                pausePanel.SetActive(true);
            }
            else if (confirmPanel.activeSelf)
            {
                confirmPanel.SetActive(false);  // 先关确认框
                TogglePausePanelButtons(true);  // ✅ 恢复交互
            }
            else if (pausePanel.activeSelf)
            {
                OnClickResumeButton();
            }
            else
            {
                if (Time.timeScale != 0) ShowPause(); // 正常打开暂停
            }
        }
    }

    public void ShowPause()
    {
        inGameUICanvas.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // ====== 主菜单按钮事件 ======
    public void OnClickResumeButton()
    {
        inGameUICanvas.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickSettingsButton()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnClickRestartConfirm()
    {
        ShowConfirm("Confirm Restart?", () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void OnClickExitConfirm()
    {
        ShowConfirm("Confirm Exit?", () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        });
    }

    // ====== 确认框控制逻辑 ======

    void ShowConfirm(string message, System.Action onYes)
    {
        confirmPanel.SetActive(true);
        confirmText.text = message;
        confirmAction = onYes;

        // ✅ 禁用 pausePanel 下所有按钮
        TogglePausePanelButtons(false);
    }

    public void OnConfirmYes()
    {
        confirmPanel.SetActive(false);
        TogglePausePanelButtons(true); // ✅ 恢复交互
        confirmAction?.Invoke();
    }

    public void OnConfirmNo()
    {
        confirmPanel.SetActive(false);
        TogglePausePanelButtons(true); // ✅ 恢复交互
    }

    /// <summary>
    /// 启用/禁用 pausePanel 下所有按钮
    /// </summary>
    void TogglePausePanelButtons(bool enable)
    {
        if (!pausePanel) return;

        Button[] buttons = pausePanel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.interactable = enable;
        }
    }
}
