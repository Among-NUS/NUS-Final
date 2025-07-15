using UnityEngine;

public class GameLevelStartup : MonoBehaviour
{
    public WorkshopSaveLoad saveLoad;

    void Start()
    {
        string jsonFile = "WorkshopLevel.json";
        Debug.Log("🔄 游戏模式加载关卡 JSON: " + jsonFile);

        if (saveLoad != null)
        {
            // ✅ 运行时 → 只加载 Prefab，没有 Wrapper
            saveLoad.LoadLayoutForGame(jsonFile);
        }
        else
        {
            Debug.LogError("❌ GameLevelStartup 缺少 WorkshopSaveLoad 引用");
        }
    }
}
