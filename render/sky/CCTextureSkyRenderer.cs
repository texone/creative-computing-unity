using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
namespace cc.creativecomputing.render
{
    public class CCTextureSkyRenderer : SkyRenderer
    {

        CCTextureSky _myTextureSky;
        Material _mySkyMaterial;

        readonly int _NoiseMap = Shader.PropertyToID("_NoiseMap");
        readonly int _Dither = Shader.PropertyToID("_Dither");
        MaterialPropertyBlock _myPropertyBlock;

        public CCTextureSkyRenderer(CCTextureSky theNoiseSky)
        {
            _myTextureSky = theNoiseSky;
            _myPropertyBlock = new MaterialPropertyBlock();
        }

        public override void Build()
        {
            Shader shader = Shader.Find("Sky/TextureSky");
            if (shader == null)
            {
                Debug.LogError("Cannot create required material because shader " + "Sky/TextureSky" + " could not be found");
                return;
            }

            _mySkyMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        public override void Cleanup()
        {
        }

        public override bool IsValid()
        {
            return _myTextureSky != null && _mySkyMaterial != null;
        }

        public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
        {
            /*
            _myPropertyBlock.SetTexture(HDShaderIDs._Cubemap, _myPropertyBlock.hdriSky);
            _myPropertyBlock.SetVector(HDShaderIDs._SkyParam, new Vector4(_myPropertyBlock.exposure, _myPropertyBlock.multiplier, -_myPropertyBlock.rotation, 0.0f)); // -rotation to match Legacy...
            */
            // This matrix needs to be updated at the draw call frequency.

            _mySkyMaterial.SetFloat(_Dither, _myTextureSky.dither.value);
            _mySkyMaterial.SetTexture(_NoiseMap, _myTextureSky.texture.value);
            //Shader.PropertyToID("_PixelCoordToViewDirWS")
            //_myPropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);
            _myPropertyBlock.SetMatrix(Shader.PropertyToID("_PixelCoordToViewDirWS"), builtinParams.pixelCoordToViewDirMatrix);
            CoreUtils.DrawFullScreen(builtinParams.commandBuffer, _mySkyMaterial, _myPropertyBlock, renderForCubemap ? 0 : 1);
        }
        
        /*
        public override void SetRenderTargets(BuiltinSkyParameters builtinParams)
        {

            CoreUtils.SetRenderTarget(builtinParams.commandBuffer, builtinParams.colorBuffer);

        }*/
    }
}
