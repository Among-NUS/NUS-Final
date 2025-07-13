using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceFonts : EditorWindow
{
    Font targetFont;                // �� UGUI Text �� TextMesh
    TMP_FontAsset targetTMPFont;    // �� TextMeshPro

    [MenuItem("Tools/�����滻���塭")]
    static void Init()
    {
        GetWindow<ReplaceFonts>("�����滻����");
    }

    void OnGUI()
    {
        GUILayout.Label("ѡ��Ҫ�滻�ɵ�������", EditorStyles.boldLabel);
        targetFont = (Font)EditorGUILayout.ObjectField("UGUI / TextMesh Font", targetFont, typeof(Font), false);
        targetTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font Asset", targetTMPFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("��ʼ�滻"))
        {
            if (!targetFont && !targetTMPFont)
            {
                EditorUtility.DisplayDialog("��ʾ", "������ָ��һ��Ŀ�����壡", "OK");
                return;
            }
            ReplaceInProject();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("���", "�����滻��ϣ�", "OK");
        }
    }

    static void ReplaceInProject()
    {
        // ֻ���� Assets/ Ŀ¼
        var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        var guids = sceneGuids.Concat(prefabGuids);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // ------------------ Prefab ------------------
            if (path.EndsWith(".prefab"))
            {
                var root = PrefabUtility.LoadPrefabContents(path);
                bool changed = ReplaceInHierarchy(root);

                if (changed)
                    PrefabUtility.SaveAsPrefabAsset(root, path);

                PrefabUtility.UnloadPrefabContents(root);
            }
            // ------------------ Scene -------------------
            else if (path.EndsWith(".unity"))
            {
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                bool changed = false;

                foreach (var rootObj in scene.GetRootGameObjects())
                    changed |= ReplaceInHierarchy(rootObj);

                if (changed)
                    EditorSceneManager.SaveScene(scene);
            }
        }
    }


    static bool ReplaceInHierarchy(GameObject root)
    {
        bool changed = false;
        var win = GetWindow<ReplaceFonts>();

        foreach (var txt in root.GetComponentsInChildren<Text>(true))
        {
            if (win.targetFont && txt.font != win.targetFont)
            {
                Undo.RecordObject(txt, "Replace Font");
                txt.font = win.targetFont;
                EditorUtility.SetDirty(txt);
                changed = true;
            }
        }
        foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (win.targetTMPFont && tmp.font != win.targetTMPFont)
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = win.targetTMPFont;
                EditorUtility.SetDirty(tmp);
                changed = true;
            }
        }
        foreach (var tm in root.GetComponentsInChildren<TextMesh>(true))
        {
            if (win.targetFont && tm.font != win.targetFont)
            {
                Undo.RecordObject(tm, "Replace 3D Text Font");
                tm.font = win.targetFont;
                EditorUtility.SetDirty(tm);
                changed = true;
            }
        }
        return changed;
    }
}
