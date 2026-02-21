using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[System.Serializable]
public class TintVolume : CustomPostProcessVolumeBase
{
    public ColorParameter tintColor = new(Color.red);
    
    public override void Render(CommandBuffer cmd, ref RenderingData renderingData, RTHandle source, RTHandle destination)
    {
        postProcessMaterial.SetColor("_TintColor", tintColor.value);
        
        Blitter.BlitCameraTexture(cmd,source,destination,postProcessMaterial,0);
    }

    public override void SetupRT(CommandBuffer cmd, ref RenderingData renderingData)
    {
        //throw new System.NotImplementedException();
    }
}