using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderPipelineSwitcher : MonoBehaviour
{
    [Header("要恢复的 URP Asset")]
    public UniversalRenderPipelineAsset urpAsset;

    UniversalRenderPipelineAsset originalURP;

    void Awake()
    {
        // 记录原始 RP（通常就是 URP）
        originalURP = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;

        // 进入编辑器场景时 → 关闭渲染管线 (None)
        GraphicsSettings.renderPipelineAsset = null;
        Debug.Log("[RenderPipelineSwitcher] 已关闭渲染管线 (None)");
    }

    void OnDestroy()
    {
        // 退出时恢复 URP（如果没设置则用之前记录的）
        GraphicsSettings.renderPipelineAsset = urpAsset != null ? urpAsset : originalURP;
        Debug.Log("[RenderPipelineSwitcher] 已恢复 URP");
    }
}
