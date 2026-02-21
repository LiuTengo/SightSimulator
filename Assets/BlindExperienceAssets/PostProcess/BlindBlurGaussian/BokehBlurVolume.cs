using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BokehBlurVolume :CustomPostProcessVolumeBase
{
    public FloatParameter  Iteration = new FloatParameter(2);
    public FloatParameter  BlurRadius = new FloatParameter(0.5f);
    public Vector2Parameter DownSample = new Vector2Parameter(Vector2.one);

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