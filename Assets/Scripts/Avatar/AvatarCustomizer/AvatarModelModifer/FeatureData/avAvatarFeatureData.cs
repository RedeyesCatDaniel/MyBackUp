using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LGUVirtualOffice
{

    public interface IAvatarFeatureData {
        public int GetIndex(FeatureGroup group);
        public List<FeatureShapeData> BlendShapeData { get; set; }
        public List<FeatureMeshData> FeatureMeshData { get; set; }
        public List<FeatureColorData> FeatureColors { get; set; }
        public List<FeatureTextureData> FeatureTextures { get; set; }
    }
    [CreateAssetMenu(menuName = "FeatureData/AvatarFeatureData", fileName = "AvatarFeatureData")]
    public class avAvatarFeatureData : ScriptableObject, IAvatarFeatureData
    {
        public List<FeatureGroup> groups;
        public List<FeatureShapeData> blendShapeData;
        public List<FeatureMeshData> featureMeshData;
        public List<FeatureColorData> colors;
        public List<FeatureTextureData> textures;

        public List<FeatureShapeData> BlendShapeData { get => blendShapeData; set => blendShapeData = value; }
        public List<FeatureMeshData> FeatureMeshData { get => featureMeshData; set => featureMeshData = value; }
        public List<FeatureColorData> FeatureColors { get => colors; set => colors = value; }
        public List<FeatureTextureData> FeatureTextures { get => textures; set => textures = value; }

        public int GetIndex(FeatureGroup group) {
            return groups.IndexOf(group);
        }
    }

    public class avAvatarFeatureDataBundle : IAvatarFeatureData
    {
        public List<FeatureGroup> groups;
        public List<FeatureShapeData> blendShapeData;
        public List<FeatureMeshData> featureMeshData;
        public List<FeatureColorData> colors;
        public List<FeatureTextureData> textures;
        public List<FeatureShapeData> BlendShapeData { get => blendShapeData; set => blendShapeData = value; }
        public List<FeatureMeshData> FeatureMeshData { get => featureMeshData; set => featureMeshData = value; }
        public List<FeatureColorData> FeatureColors { get => colors; set => colors = value; }
        public List<FeatureTextureData> FeatureTextures { get => textures; set => textures = value; }

        public int GetIndex(FeatureGroup group)
        {
            throw new System.NotImplementedException();
        }
    }

    [System.Serializable]
    public class FeatureShapeData {
        
        public List<float> data = new List<float>();
        public List<string> indexes = new List<string>();


        

        public void Write(string index, float value) {
            int target = indexes.IndexOf(index);
            if (target > -1)
            {
                data[target] = value;
            }
            else {
                data.Add(value);
                indexes.Add(index);
            }
            
        }
    }

    [System.Serializable]
    public class FeatureMeshData {
        public List<string> boneNames = new List<string>();
        public AssetReference mesh;
        public void Write(SkinnedMeshRenderer renderer) {
            boneNames.Clear();
            foreach (var item in renderer.bones)
            {
                boneNames.Add(item.name);
                
            }
        }

        public void Write(AssetReference mesh) {
            this.mesh = mesh;
        }
       
    }

    [System.Serializable]
    public class FeatureColorData {
        public List<string> colorName;
        public List<Color> colors;
    }

    [System.Serializable]
    public class FeatureTextureData {
        public List<string> textName;
        public List<AssetReference> texture;
    }


}