using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurPostProcessVolume :CustomPostProcessVolumeBase
{
    public FloatParameter blurRadius = new FloatParameter(0.02f);
    public FloatParameter blurIteration = new FloatParameter(1f);
    
    //discard
    public override void Render(CommandBuffer cmd, ref RenderingData renderingData, RTHandle source, RTHandle destination)
    {
        throw new System.NotImplementedException();
    }

    public override void SetupRT(CommandBuffer cmd, ref RenderingData renderingData)
    {
        throw new System.NotImplementedException();
    }
}