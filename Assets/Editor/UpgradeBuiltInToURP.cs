using UnityEditor;
using UnityEngine;

public class UpgradeBuiltInToURP
{
    [MenuItem("Tools/Upgrade Built-in Materials to URP")]
    static void UpgradeMaterials()
    {
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");

        int changed = 0;
        foreach (var matGUID in AssetDatabase.FindAssets("t:Material"))
        {
            string path = AssetDatabase.GUIDToAssetPath(matGUID);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat.shader.name == "Standard")
            {
                mat.shader = urpLit;
                changed++;
                Debug.Log($"✅ 升级 {mat.name} → URP/Lit");
            }
            else if (mat.shader.name.Contains("Unlit"))
            {
                mat.shader = urpUnlit;
                changed++;
                Debug.Log($"✅ 升级 {mat.name} → URP/Unlit");
            }
        }

        Debug.Log($"🎉 完成升级，共修改 {changed} 个材质");
    }
}
