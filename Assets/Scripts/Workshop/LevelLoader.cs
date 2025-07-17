// ─────────────────────────────────────────────────────────────
// LevelLoader.cs
// 运行时从 JSON 加载关卡数据到游戏场景（与编辑器解耦）
// ─────────────────────────────────────────────────────────────
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelDataBridge
{
    public static string pendingLevelJsonPath;
}

public class LevelLoader : MonoBehaviour
{
    [Header("UI 输入")]
    public TMP_InputField fileNameField;  // ✅ 从输入框获取路径

    [Tooltip("加载后跳转的正式游戏场景名")]
    public string targetSceneName = "GameLevelScene";

    // ✅ 自动补 .ana 并默认桌面路径
    static string ResolvePath(string f)
    {
        if (string.IsNullOrEmpty(f))
            f = "WorkshopLevel.ana";

        if (!f.EndsWith(".ana"))
            f += ".ana";

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        return Path.IsPathRooted(f)
            ? f
            : Path.Combine(desktopPath, f);
    }

    public void LoadLevelInGameScene()
    {
        // ✅ 先取输入框内容，如果为空就用默认
        string input = (fileNameField != null && !string.IsNullOrWhiteSpace(fileNameField.text))
                        ? fileNameField.text.Trim()
                        : "WorkshopLevel.ana";

        string fullPath = ResolvePath(input);

        if (!File.Exists(fullPath))
        {
            Debug.LogError("❌ 找不到关卡文件: " + fullPath);
            return;
        }

        // ✅ 保存到桥接类
        LevelDataBridge.pendingLevelJsonPath = fullPath;

        // ✅ 跳转场景
        SceneManager.LoadScene(targetSceneName);
    }
}
