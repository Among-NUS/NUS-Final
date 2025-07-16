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

        // 1️⃣ 找到场景中可能存在的 Hero(Clone)
        GameObject heroClone = GameObject.Find("Hero(Clone)");
        if (heroClone)
        {
            Vector3 clonePos = heroClone.transform.position;
            Quaternion cloneRot = heroClone.transform.rotation;

            Debug.Log($"[HeroInitializer] 找到 Hero(Clone)，位置 {clonePos}");

            // 2️⃣ 删除 Clone
            Destroy(heroClone);

            // 3️⃣ 找到场景里原始的 Hero
            GameObject heroOriginal = GameObject.Find("Hero");
            if (heroOriginal)
            {
                heroOriginal.transform.position = clonePos;
                heroOriginal.transform.rotation = cloneRot;

                Debug.Log("[HeroInitializer] 已移动 Hero 到 Clone 位置");
            }
            else
            {
                Debug.LogWarning("[HeroInitializer] 没找到原始 Hero！");
            }
        }
        else
        {
            Debug.Log("[HeroInitializer] 场景里没有 Hero(Clone)，不做处理");
        }

    }
}
