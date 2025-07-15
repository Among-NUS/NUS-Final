// ─────────────────────────────────────────────────────────────
// LevelLoader.cs
// 运行时从 JSON 加载关卡数据到游戏场景（与编辑器解耦）
// ─────────────────────────────────────────────────────────────
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelDataBridge
{
    public static string pendingLevelJsonPath;
}

public class LevelLoader : MonoBehaviour
{
    [Tooltip("你保存好的 JSON 文件相对路径，如 Assets/Scenes/WorkshopLevel.json")]
    public string levelJsonRelativePath = "Assets/Scenes/WorkshopLevel.json";

    [Tooltip("加载后跳转的正式游戏场景名")]
    public string targetSceneName = "GameLevelScene";

    public void LoadLevelInGameScene()
    {
        string fullPath = System.IO.Path.Combine(Application.dataPath, "Scenes/" + System.IO.Path.GetFileName(levelJsonRelativePath));
        if (!System.IO.File.Exists(fullPath))
        {
            Debug.LogError("找不到关卡文件: " + fullPath);
            return;
        }

        // 设置待加载路径
        LevelDataBridge.pendingLevelJsonPath = fullPath;

        // 加载游戏场景
        SceneManager.LoadScene(targetSceneName);
    }
}