using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderPipelineSwitcher : MonoBehaviour
{
    [Header("Ҫ�ָ��� URP Asset")]
    public UniversalRenderPipelineAsset urpAsset;

    UniversalRenderPipelineAsset originalURP;

    void Awake()
    {
        // ��¼ԭʼ RP��ͨ������ URP��
        originalURP = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;

        // ����༭������ʱ �� �ر���Ⱦ���� (None)
        GraphicsSettings.renderPipelineAsset = null;
        Debug.Log("[RenderPipelineSwitcher] �ѹر���Ⱦ���� (None)");
    }

    void OnDestroy()
    {
        // �˳�ʱ�ָ� URP�����û��������֮ǰ��¼�ģ�
        GraphicsSettings.renderPipelineAsset = urpAsset != null ? urpAsset : originalURP;
        Debug.Log("[RenderPipelineSwitcher] �ѻָ� URP");
    }
}
