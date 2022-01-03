using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace LGUVirtualOffice
{
    public static class BlendShapeDataMaker
    {
        public static void GenerateBlendShapeData(this avModifierDataCollection data) {
            data.blendShapeModifiers.modifiers.kpv.Clear();
            string prefix = "blendShape1.";
            string[] type = new string[] { "face","eyes","mouth","nose"};


            foreach (var item in type)
            {
                for (int i = 0; i < 10; i++)
                {
                    string keyName = item + i.ToString();
                    avBlendShapeModifier absm = new avBlendShapeModifier();
                    absm.TargetModifier = "body";
                    data.blendShapeModifiers.modifiers.kpv.Add(new avPair<string, avBlendShapeModifier>(keyName, absm));
                    for (int x = 1; x < 4; x++)
                    {
                        for (int y = 1; y < 4; y++)
                        {
                            string key = prefix + item + x + "_" + y;
                            int val = (x-1)*3 + y == i ? 100 : 0;
                            absm.blendshapes.kpv.Add(new avPair<string, float>(key,val));
                        }
                    }
                }
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(data);
#endif




        }
    }
}