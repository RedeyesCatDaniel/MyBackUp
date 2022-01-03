using System;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Universal
{
    static class URPUtils
    {

        public static float Asfloat(uint val) { unsafe { return *((float*)&val); } }

        // These two convertion functions are used to store GUID assets inside materials,
        // a unity asset GUID is exactly 16 bytes long which is also a Vector4 so by adding a
        // Vector4 field inside the shader we can store references of an asset inside the material
        // which is actually used to store the reference of the diffusion profile asset
        internal static Vector4 ConvertGUIDToVector4(string guid)
        {
            Vector4 vector;
            byte[] bytes = new byte[16];

            for (int i = 0; i < 16; i++)
                bytes[i] = byte.Parse(guid.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);

            unsafe
            {
                fixed (byte* b = bytes)
                    vector = *(Vector4*)b;
            }

            return vector;
        }

        internal static string ConvertVector4ToGUID(Vector4 vector)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            unsafe
            {
                byte* v = (byte*)&vector;
                for (int i = 0; i < 16; i++)
                    sb.Append(v[i].ToString("x2"));
                var guidBytes = new byte[16];
                System.Runtime.InteropServices.Marshal.Copy((IntPtr)v, guidBytes, 0, 16);
            }

            return sb.ToString();
        }
    }
}