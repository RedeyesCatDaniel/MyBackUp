using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Assertions;

namespace UnityEditor.Rendering.Universal
{
    using UnityObject = UnityEngine.Object;

    class CharacterShaderMenuItems
    {
        class DoCreateNewAsset<TAssetType> : ProjectWindowCallback.EndNameEditAction where TAssetType : ScriptableObject
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var newAsset = CreateInstance<TAssetType>();
                newAsset.name = Path.GetFileName(pathName);
                AssetDatabase.CreateAsset(newAsset, pathName);
                ProjectWindowUtil.ShowCreatedAsset(newAsset);
                PostCreateAssetWork(newAsset);
            }

            protected virtual void PostCreateAssetWork(TAssetType asset) { }
        }

        class DoCreateNewAssetDiffusionProfileSettings : DoCreateNewAsset<DiffusionProfileSettings>
        {
            protected override void PostCreateAssetWork(DiffusionProfileSettings asset)
            {
                // Update the hash after that the asset was saved on the disk (hash requires the GUID of the asset)
                DiffusionProfileHashTable.UpdateDiffusionProfileHashNow(asset);
            }
        }

        [MenuItem("Assets/Create/Rendering/Diffusion Profile", priority = CoreUtils.Sections.section4 + CoreUtils.Priorities.assetsCreateRenderingMenuPriority)]
        static void MenuCreateDiffusionProfile()
        {
            var icon = EditorGUIUtility.FindTexture("ScriptableObject Icon");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateNewAssetDiffusionProfileSettings>(), "New Diffusion Profile.asset", icon, null);
        }

    }
}