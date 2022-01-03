using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEngine;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering.Universal;
using UnityEditor.ShaderGraph;
using System.Globalization;
using static UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers.ShaderInputPropertyDrawer;

// AbstractShaderProperty --> PackageCache\com.unity.shadergraph\Editor\Data\Graphs

namespace UnityEditor.Rendering.Universal//.ShaderGraph
{
    [Serializable]
    [BlackboardInputInfo(55)]
    class DiffusionProfileShaderProperty : AbstractShaderProperty<LazyLoadReference<SubsurfaceScatteringProfile>>, IShaderPropertyDrawer
    {
        internal DiffusionProfileShaderProperty()
        {
            displayName = "Diffusion Profile";
        }

        internal override bool isExposable => true;
        internal override bool isRenamable => true;

        public override PropertyType propertyType => PropertyType.Float;

        string assetReferenceName => $"{referenceName}_Asset";

        Vector4 ConvertGUIDToVector4(string guid)
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

        float Asfloat(uint val) { unsafe { return *((float*)&val); } }

        internal override string GetPropertyBlockString()
        {
            uint hash = 0;
            Vector4 asset = Vector4.zero;

            if (value.isSet)
            {
                hash = value.asset.hash;
                asset = ConvertGUIDToVector4(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value.asset)));
            }

            /// <summary>Float to string convertion function without any loss of precision</summary>
            string f2s(float f) => System.Convert.ToDouble(f).ToString("0." + new string('#', 339));

            return
$@"[DiffusionProfile]{referenceName}(""{displayName}"", Float) = {f2s(Asfloat(hash))}
[HideInInspector]{assetReferenceName}(""{displayName}"", Vector) = ({f2s(asset.x)}, {f2s(asset.y)}, {f2s(asset.z)}, {f2s(asset.w)})";
        }

        public override string GetOldDefaultReferenceName() => $"DiffusionProfile_{objectId}";


        internal override string GetPropertyAsArgumentString(string precisionString)
        {
            return $"float {referenceName}";
        }

        internal override void ForeachHLSLProperty(Action<HLSLProperty> action)
        {
            HLSLDeclaration decl = GetDefaultHLSLDeclaration();
            action(new HLSLProperty(HLSLType._float, referenceName, decl));
        }

        internal override AbstractMaterialNode ToConcreteNode()
        {
            var node = new DiffusionProfileNode();
            node.diffusionProfile = value.asset;
            return node;
        }

        internal override PreviewProperty GetPreviewMaterialProperty()
        {
            return new PreviewProperty(propertyType)
            {
                name = referenceName,
                floatValue = value.isSet ? Asfloat(value.asset.hash) : 0
            };
        }

        internal override ShaderInput Copy()
        {
            return new DiffusionProfileShaderProperty()
            {
                displayName = displayName,
                hidden = hidden,
                value = value,
                precision = precision,
                overrideHLSLDeclaration = overrideHLSLDeclaration,
                hlslDeclarationOverride = hlslDeclarationOverride
            };
        }

        void IShaderPropertyDrawer.HandlePropertyField(PropertySheet propertySheet, PreChangeValueCallback preChangeValueCallback, PostChangeValueCallback postChangeValueCallback)
        {
            var diffusionProfileDrawer = new DiffusionProfilePropertyDrawer();

            propertySheet.Add(diffusionProfileDrawer.CreateGUI(
                newValue => {
                    preChangeValueCallback("Changed Diffusion Profile");
                    value = newValue;
                    postChangeValueCallback(true);
                },
                value.asset,
                "Diffusion Profile",
                out var _));
        }

        public override int latestVersion => 1;
        public override void OnAfterDeserialize(string json)
        {
            if (sgVersion == 0)
            {
                LegacyShaderPropertyData.UpgradeToHLSLDeclarationOverride(json, this);
                ChangeVersion(1);
            }
        }
    }
}
