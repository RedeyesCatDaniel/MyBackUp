
// Float
void PhysicallyBasedRefraction_float(
    float mask, float3 viewW, half3 normalW,
    float eyeAnteriorChamberDepth, float eyeIOR, float eyeRadius, float3 eyeLookVector,
    out float2 uvOffset)
{
    half3 normal = normalW;

    float height = eyeAnteriorChamberDepth * saturate(1.0 - 18.4 * eyeRadius * eyeRadius);

    // Calculate Physical Based Refraction
    float w = eyeIOR * dot(normal, viewW);
    float k = sqrt(1.0 + (w - eyeIOR) * (w + eyeIOR));
    float3 refractedW = (w - k) * normal - eyeIOR * viewW;

    // Ref: Next-Generation Character Rendering GDC 2013 #233
    float cosAlpha = dot(eyeLookVector, -refractedW);
    float dist = height / cosAlpha;
    float3 offsetW = dist * refractedW;

    //NOTE: Right now the effect is correct when multiplying by the model matrix, and negating the look vector.
    //      Mathematically, we should be transforming the offset into texture space.
    float2 offsetT = mul(offsetW, (float3x3)unity_ObjectToWorld).xy;

    // UV is flip in X axis
    uvOffset = float2(-mask, mask) * offsetT;
}

// Half
void PhysicallyBasedRefraction_half(
    half mask, half3 viewW, half3 normalW,
    half eyeAnteriorChamberDepth, half eyeIOR, half eyeRadius, half3 eyeLookVector,
    out float2 uvOffset)
{
    half3 normal = normalW;

    half height = eyeAnteriorChamberDepth * saturate(1.0 - 18.4 * eyeRadius * eyeRadius);

    // Calculate Physical Based Refraction
    half w = eyeIOR * dot(normal, viewW);
    half k = sqrt(1.0 + (w - eyeIOR) * (w + eyeIOR));
    half3 refractedW = (w - k) * normal - eyeIOR * viewW;

    // Ref: Next-Generation Character Rendering GDC 2013 #233
    half cosAlpha = dot(eyeLookVector, -refractedW);
    half dist = height / cosAlpha;
    half3 offsetW = dist * refractedW;

    //NOTE: Right now the effect is correct when multiplying by the model matrix, and negating the look vector.
    //      Mathematically, we should be transforming the offset into texture space.
    float2 offsetT = mul(offsetW, (half3x3)unity_ObjectToWorld).xy;

    // UV is flip in X axis
    uvOffset = float2(-mask, mask) * offsetT;
}