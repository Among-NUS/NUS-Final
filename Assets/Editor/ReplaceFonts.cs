using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

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
        // �������г����� prefab
        string[] guids = AssetDatabase.FindAssets("t:Scene t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            bool changed = false;

            // Prefab
            if (path.EndsWith(".prefab"))
            {
                var root = PrefabUtility.LoadPrefabContents(path);
                changed = ReplaceInHierarchy(root) || changed;
                if (changed)
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                PrefabUtility.UnloadPrefabContents(root);
            }
            // Scene
            else if (path.EndsWith(".unity"))
            {
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                foreach (var rootObj in scene.GetRootGameObjects())
                    changed = ReplaceInHierarchy(rootObj) || changed;

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
