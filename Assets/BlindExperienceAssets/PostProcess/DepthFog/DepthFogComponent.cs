using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



public class DepthFogComponent : VolumeComponent
{
    public ColorParameter fogColor = new ColorParameter(Color.black);

    public FloatParameter startDistance = new FloatParameter(5.0f);

    public FloatParameter endDistance = new FloatParameter(30f);

    public FloatParameter density = new FloatParameter(1f);
    
    private Material DepthFogMaterial;
}

public class DepthFogPass : FullscreenPassBase
{
    private string passName = "DepthFogPass";
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var volumeComponent = VolumeManager.instance.stack.GetComponent<DepthFogComponent>();

        //*Set Param
        material.SetColor("_FogColor", volumeComponent.fogColor.value);
        material.SetFloat("_Start", volumeComponent.startDistance.value);
        material.SetFloat("_End", volumeComponent.endDistance.value);
        material.SetFloat("_Density", volumeComponent.density.value);
        //
        
        base.Execute(context, ref renderingData);
    }
}