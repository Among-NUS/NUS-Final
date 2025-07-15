// ��������������������������������������������������������������������������������������������������������������������������
// LevelLoader.cs
// ����ʱ�� JSON ���عؿ����ݵ���Ϸ��������༭�����
// ��������������������������������������������������������������������������������������������������������������������������
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelDataBridge
{
    public static string pendingLevelJsonPath;
}

public class LevelLoader : MonoBehaviour
{
    [Tooltip("�㱣��õ� JSON �ļ����·������ Assets/Scenes/WorkshopLevel.json")]
    public string levelJsonRelativePath = "Assets/Scenes/WorkshopLevel.json";

    [Tooltip("���غ���ת����ʽ��Ϸ������")]
    public string targetSceneName = "GameLevelScene";

    public void LoadLevelInGameScene()
    {
        string fullPath = System.IO.Path.Combine(Application.dataPath, "Scenes/" + System.IO.Path.GetFileName(levelJsonRelativePath));
        if (!System.IO.File.Exists(fullPath))
        {
            Debug.LogError("�Ҳ����ؿ��ļ�: " + fullPath);
            return;
        }

        // ���ô�����·��
        LevelDataBridge.pendingLevelJsonPath = fullPath;

        // ������Ϸ����
        SceneManager.LoadScene(targetSceneName);
    }
}