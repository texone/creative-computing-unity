struct Voxel{
    float3 position;
    //float4 color;
};

StructuredBuffer <Voxel> voxelBuffer; 

float4x4 mat;
float3 position;

int width;
int height;
int offset;
float spacing;
float size;

PackedVaryingsType InstancedVert(
    AttributesMesh inputMesh,
    uint instanceID : SV_instanceID
){

    Voxel v = voxelBuffer[instanceID];

    // Instance Object Space
    float3 pos = inputMesh.positionOS;
    pos *= size;
    
    // id 
    float3 id = 0;
    id.x = instanceID % width;
    id.z = floor(instanceID / (float)width);
    
    // get color
    //float4 c = v.color;
    //c = clamp(c,0,1);
    float4 c = float4(1,0,0,1);
    float3 vpos = v.position;// - float3(width / 2.0, 0, width / 2.0);
    vpos.y += offset;
    //vpos *= spacing; 
    
    // xz scale
    //pos.xz *= size* float(c.r > 0.25);
    
    // set position
    inputMesh.positionOS = mul(mat,pos + vpos).xyz + position;
    
    VaryingsType vt;
    vt.vmesh = VertMesh(inputMesh);
    return PackVaryingsType(vt); 
}

void InstancedFrag(
    PackedVaryingsToPS packedInput,
	OUTPUT_GBUFFER(outGBuffer)
#ifdef _DEPTHOFFSET_ON
	, out float outputDepth : SV_Depth
#endif
)
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
	FragInputs input = UnpackVaryingsMeshToFragInputs(packedInput.vmesh);

	// input.positionSS is SV_Position
	PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

#ifdef VARYINGS_NEED_POSITION_WS
	float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
#else
	// Unused
	float3 V = float3(1.0, 1.0, 1.0); // Avoid the division by 0
#endif

	SurfaceData surfaceData;
	BuiltinData builtinData;
	GetSurfaceAndBuiltinData(input, V, posInput, surfaceData, builtinData);


	///////////////////////////////////////////////
	// Workshop Customize
	surfaceData.baseColor = input.color.rgb;
	///////////////////////////////////////////////


	ENCODE_INTO_GBUFFER(surfaceData, builtinData, posInput.positionSS, outGBuffer);

#ifdef _DEPTHOFFSET_ON
	outputDepth = posInput.deviceDepth;
#endif
}