using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EyeOcclusionUV : MonoBehaviour
{
    Material eyeMat;
    MeshRenderer meshRenderer;

    private void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            eyeMat = meshRenderer.sharedMaterial;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(eyeMat != null)
        {
            if (transform.parent != null)
                eyeMat.SetMatrix("_UV_transform", transform.parent.worldToLocalMatrix);
            else
                eyeMat.SetMatrix("_UV_transform", Matrix4x4.identity);
        }
    }
}
