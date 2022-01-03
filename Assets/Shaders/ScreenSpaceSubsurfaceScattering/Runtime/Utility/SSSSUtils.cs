using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SSSSUtils
    {

        public static Material CreateEngineMaterial(string shaderPath)
        {
            var mat = new Material(Shader.Find(shaderPath))
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            return mat;
        }

        public static CommandBuffer PrepareBuffer(Camera cam, CameraEvent e, string cmdName)
        {
            CommandBuffer cmd = null;
            CommandBuffer[] existing;
            if ((existing = cam.GetCommandBuffers(e)).Length > 0)
            {
                cmd = existing[0];
                cmd.Clear();
            }
            else
            {
                cmd = new CommandBuffer();
                cmd.name = cmdName;
                cam.AddCommandBuffer(e, cmd);
            }
            return cmd;
        }

        public static Matrix4x4 GetGPUProjInverse(Camera cam)
        {
            return (GL.GetGPUProjectionMatrix(cam.projectionMatrix, false)).inverse;
        }

        public static void SelectKeyword(Material material, string keyword1, string keyword2, bool enableFirst)
        {
            material.EnableKeyword(enableFirst ? keyword1 : keyword2);
            material.DisableKeyword(enableFirst ? keyword2 : keyword1);
        }

        public static void SelectKeyword(Material material, string[] keywords, int enabledKeywordIndex)
        {
            material.EnableKeyword(keywords[enabledKeywordIndex]);

            for (int i = 0; i < keywords.Length; i++)
            {
                if (i != enabledKeywordIndex)
                {
                    material.DisableKeyword(keywords[i]);
                }
            }
        }

    }
