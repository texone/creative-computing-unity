Texture2D<float4> tex;

float4x4 mat;
float3 position;

int width;
int height;
float spacing;
float size;
float sizeY;

PackedVaryingsType InstancedVert(
    AttributesMesh inputMesh,
    uint instanceID : SV_instanceID
){
    // Instance Object Space
    float4 pos = 0;
    pos.xyz = inputMesh.positionOS;
    
    // id 
    float3 id = 0;
    id.x = instanceID % width;
    id.z = floor(instanceID / (float)width);
    
    // get color
    float4 c = tex[id.xz];
    c = clamp(c,0,1);
    
    // height
    float height = sizeY * (c.r) * 20;
    pos.y *= height;
    pos.y += height / 2.0;
    
    // Set Color
#ifdef ATTRIBUTES_NEED_COLOR 
     inputMesh.color = c.r;
#endif

    // grid position 
    float4 gpos = 0;
    gpos.xz = id.xz - int2(width, height) / 2.0;
    gpos.y = 0;
    gpos *= spacing;
    
    // xz scale
    //pos.xz *= size* float(c.r > 0.25);
    
    // set position
    inputMesh.positionOS = mul(mat,pos + gpos).xyz + position;
    
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