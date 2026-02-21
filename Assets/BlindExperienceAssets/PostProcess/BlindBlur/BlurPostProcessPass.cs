using System.Data.OleDb;
using System.Net.Configuration;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class BlurPostProcessPass : ScriptableRenderPass{
    // 给profiler入一个新的事件
    private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("ColorBlit");
    private Material m_Material;
    // RTHandle，封装了纹理及相关信息，可以认为是CPU端纹理
    private RTHandle m_CameraColorTarget;
    private RTHandle tempRT;
        
    private float m_BlurRadius;
    private float m_Iteration;

    public BlurPostProcessPass(Material material) {
        m_Material = material;
        // 指定执行这个Pass的时机
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    // 指定进行后处理的target
    public void SetTarget(RTHandle colorHandle) {
        m_CameraColorTarget = colorHandle;
    }

    // OnCameraSetup是纯虚函数，相机初始化时调用
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
        // (父类函数)指定pass的render target
        ConfigureTarget(m_CameraColorTarget);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        ConfigureInput(ScriptableRenderPassInput.Color);
    }

    // Execute时抽象函数，把cmd命令添加到context中（然后进一步送到GPU调用）
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        var cameraData = renderingData.cameraData;
        if (cameraData.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        var stack = VolumeManager.instance.stack;
        var BlurPPComponent = stack.GetComponent<BlurPostProcessVolume>();
        if (BlurPPComponent == null)
        {
            return;
        }
        else
        {
            m_BlurRadius = BlurPPComponent.blurRadius.value;
            m_Iteration = BlurPPComponent.blurIteration.value;
        }

        if (!BlurPPComponent.IsActive())
        {
            return;
        }
        
        // 获取commandbuffer
        CommandBuffer cmd = CommandBufferPool.Get();
        // 把cmd里执行的命令添加到m_ProfilingSampler定义的profiler块中
        // using用来自动释放new的资源
        m_Material.SetFloat("_BlurRadius", m_BlurRadius);
        m_Material.SetFloat("_Iteration", m_Iteration);
        // 使用cmd里的命令(设置viewport等，分辨率等），执行m_Material的pass0，将m_CameraColorTarget渲染到m_CameraColorTarget
        // 本质上画了一个覆盖屏幕的三角形
        if (!m_Material)
        {
            Debug.LogWarning("BlurPostProcessPass requires a material");
        }

        var desc = renderingData.cameraData.cameraTargetDescriptor;
        desc.depthBufferBits = 0;   // 不能带 depth
        desc.msaaSamples = 1;       // 不能带 MSAA
        desc.bindMS = false;
        RenderingUtils.ReAllocateIfNeeded(ref tempRT,in desc);
        
        Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, tempRT);
        Blitter.BlitCameraTexture(cmd,tempRT,m_CameraColorTarget,m_Material,0);

        // 把cmd中的命令入到context中
        context.ExecuteCommandBuffer(cmd);
        // 清空cmd栈
        cmd.Clear();
        
        CommandBufferPool.Release(cmd);
    }
}
