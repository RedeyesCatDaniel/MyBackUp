#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
namespace LGUVirtualOffice
{
    public class avCharacterCustomizationSetup : MonoBehaviour
    {
        public AvatarCustomizationUISetting[] settings;
        public List<avAvatarRenderer> created;


        [ContextMenu("Init")]
        public void Init()
        {
          //  manager.myBodies = new avAvatarRenderer[settings.Length];
            foreach (var item in created)
            {
                DestroyImmediate(item.gameObject);
            }
            created = new List<avAvatarRenderer>();
            for (int i = 0; i < settings.Length; i++)
            {
                settings[i].Init(out avAvatarRenderer rs);
                //manager.myBodies[i].renderer = rs;
                created.Add(rs);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());


        }
    }

    [System.Serializable]
    public struct AvatarCustomizationUISetting {
        public avAvatarPlaceHolderManager holder;
        public avAvatarRenderer renderer;

        public void Init(out avAvatarRenderer rs) {
            rs = MonoBehaviour.Instantiate(renderer, holder.transform);
            holder.renderer = rs;
            
        }

       
    }


}
#endif