using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

internal class BokehBlurRendererFeature : ScriptableRendererFeature{
    public Shader m_Shader;
    
    private Material m_Material;

    private BokehBlurPass m_RenderPass = null;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (renderingData.cameraData.cameraType == CameraType.Game)
            // Pass入队
            renderer.EnqueuePass(m_RenderPass);
    } 

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
        // 只对游戏摄像机应用后处理（还有预览摄像机等）
        if (renderingData.cameraData.cameraType == CameraType.Game) {
            // 设置向pass输入color (m_RenderPass父类)
            m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            // 设置RT为相机的color
            m_RenderPass.SetTarget(renderer.cameraColorTargetHandle);
        }
    }

    // 基类的抽象函数 OnEnable和OnValidate时调用
    public override void Create() {
        // 创建一个附带m_Shader的material
        m_Material = CoreUtils.CreateEngineMaterial("BlurShader/BokehBlur");
        // 创建BiltPass脚本实例
        m_RenderPass = new BokehBlurPass(m_Material);
    }

    protected override void Dispose(bool disposing) {
        CoreUtils.Destroy(m_Material);
    }
}