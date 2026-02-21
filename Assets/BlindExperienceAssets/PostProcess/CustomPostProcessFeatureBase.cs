using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using BoolParameter = UnityEngine.Rendering.BoolParameter;

//RenderFeature Definition
public class CustomPostProcessFeatureBase : ScriptableRendererFeature
{

    #region Render Pass Definition

    class CustomPostProcessPass : ScriptableRenderPass
    {
        //RendererFeatureEvent
        private RenderPassEvent RenderPassEvent;
        
        //Material
        private Material PostProcessMaterial;
        
        //VolumeComponent
        public CustomPostProcessVolumeBase Volume;
        
        //RTHandles
        private RTHandle src;
        private RTHandle dest;
        private RTHandle tempRT;
        
        public void SetupRenderTarget(RTHandle target)
        {
            src = target;
            dest = target;
            //dest = destination;
        }
        
        public CustomPostProcessPass(RenderPassEvent renderPassEvent,Material material)
        {
            this.renderPassEvent = renderPassEvent;
            PostProcessMaterial = material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            descriptor.msaaSamples = 1;
            
            RenderingUtils.ReAllocateIfNeeded(ref tempRT, descriptor);
            
            
            ConfigureTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("CustomPostProcess");
            
            Blitter.BlitCameraTexture(cmd,src,tempRT);
            
            Volume?.Render(cmd, ref renderingData, tempRT,dest);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    #endregion
    
    
    //RenderFeature Parameters
    private CustomPostProcessPass m_ScriptablePass;
    private CustomPostProcessVolumeBase m_VolumeComponent;

    //Settings
    public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    public ScriptableRenderPassInput RenderPassInput = ScriptableRenderPassInput.Color;
    public Shader PostProcessShader;
    
    //RT
    private RTHandle src;

    /// <summary>
    /// Override this! return compared VolumeComponent
    /// </summary>
    /// <returns></returns>
    
    public override void Create()
    {
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        base.SetupRenderPasses(renderer, in renderingData);

        if (PostProcessShader == null)
        {
            return;
        }
        
        Material material = CoreUtils.CreateEngineMaterial(PostProcessShader);
        m_ScriptablePass = new CustomPostProcessPass(RenderPassEvent,material);

        var stack = VolumeManager.instance.stack;
        m_VolumeComponent = stack.GetComponent<TintVolume>();
        if (m_VolumeComponent == null)
        {
            return;
        }
        m_VolumeComponent.SetPostProcessMaterial(material);
        m_ScriptablePass.Volume = m_VolumeComponent;
        
        // Configures where the render pass should be injected.
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            var rend = renderingData.cameraData.renderer;
            src = rend.cameraColorTargetHandle;
            m_ScriptablePass.SetupRenderTarget(src);
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.postProcessEnabled)
        {
            m_ScriptablePass.ConfigureInput(RenderPassInput);
            
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}


public abstract class CustomPostProcessVolumeBase : VolumeComponent,IPostProcessComponent
{
    public BoolParameter enabled = new BoolParameter(false);
    
    protected Material postProcessMaterial;

    public void SetPostProcessMaterial(Material material)
    {
        postProcessMaterial = material;
    }
    
    public abstract void Render(CommandBuffer cmd, ref RenderingData renderingData, RTHandle source,RTHandle destination);
    
    public abstract void SetupRT(CommandBuffer cmd, ref RenderingData renderingData);
    
    public bool IsActive()
    {
        return enabled.value;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}