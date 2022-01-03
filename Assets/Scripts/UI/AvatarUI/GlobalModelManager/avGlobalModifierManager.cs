using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avGlobalModifierManager : MonoBehaviour,netISymchronizable
    {
        public static bool Initialized;
        public static Dictionary<string, IAvatarModifier> modifiers = new Dictionary<string, IAvatarModifier>();
        public static Dictionary<string, IAvatarModifier> lowPolyModifiers = new Dictionary<string, IAvatarModifier>();


        public static Dictionary<FeatureGroup, Color> colorModifiers = new Dictionary<FeatureGroup, Color>();       
        public static Dictionary<FeatureGroup, string> modificationsOnCharacter = new Dictionary<FeatureGroup, string>();
        private Dictionary<FeatureGroup, string>[] temp = new Dictionary<FeatureGroup, string>[2];

        
        public static int gender;

        public static avGlobalModifierManager instance;
        public avModifierDataCollection modifierData;


        public UnityEvent DOnFinishPush;
        public int Gender { get => gender; set {
                
                OnSetGender(value);
            
        }}

        [HideInInspector]
        public avAvatarRenderer myRenderer;


        private void OnSetGender(int genderValue) {
            
            if (genderValue!=gender) {
                temp[gender] = modificationsOnCharacter;

                gender = genderValue;
                modificationsOnCharacter = temp[gender];
                if (modificationsOnCharacter == null) {
                    modificationsOnCharacter = new Dictionary<FeatureGroup, string>();
                }
                Debug.Log($"My gender now is {gender}");

                
                
            }
            gender = genderValue;


        }

        

        public static bool TryGetInstance(out avGlobalModifierManager man) {
            man = instance;
            return true;
        }
        private void Awake()
        {
            if (!Initialized)
            {
                instance = this;
                modifierData.InjectData(modifiers);
                modifierData.InjectLowPolyData(lowPolyModifiers);
                Initialized = true;
            }
        }

        public Dictionary<FeatureGroup, List<Color>> GetColorByFeatureGroup() {
            return modifierData.colorModifiers.GetColorByFeatureGroup();
        }

        public static bool TryGetModifier(string id, out IAvatarModifier modifier) {
            if (modifiers.TryGetValue(id, out modifier))
            {
                return true;
            }

            Debug.Log($"Cannot find {id}");
            return false;
            
        }

        public static bool TryGetLowPolyModifier(string id, out IAvatarModifier modifier)
        {
            if (lowPolyModifiers.TryGetValue(id, out modifier))
            {
                return true;
            }
            else {
                return TryGetModifier(id,out modifier);
            }

            

        }


        //This function will modify target renderer according to the setting of global modifiers

        public static void Modify(avAvatarRenderer renderer) {
            foreach (var item in modificationsOnCharacter.Keys)
            {
                string modName = modificationsOnCharacter[item];
                if (TryGetModifier(modName, out IAvatarModifier modifier)) {
                    modifier.Modify(renderer);
                }
                
            }

            foreach (var item in colorModifiers)
            {
                FeatureGroup group = item.Key;
                Color c = item.Value;
                renderer.ChangeColor(group,c);
            }

        }

        public static void Modify(avAvatarRenderer renderer, IEnumerable<string> ModifierIds)
        {
            foreach (var item in ModifierIds)
            {
                string modName = item;
                if (TryGetModifier(modName, out IAvatarModifier modifier))
                {
                    modifier.Modify(renderer);
                }

            }

        }

        public static void LPModify(avAvatarRenderer renderer, IEnumerable<string> ModifierIds)
        {
            foreach (var item in ModifierIds)
            {
                string modName = item;
                if (TryGetLowPolyModifier(modName, out IAvatarModifier modifier))
                {
                    modifier.Modify(renderer);
                }

            }

        }

        public void Modify() {
            Modify(myRenderer);
        }

        public static void UpdateAvatarData(AvatarData data) {
            modificationsOnCharacter = SingleModifierID. GetSingleModifierIDs(data.featureModifications);
         //   colorModification = SingleModifierID.GetSingleModifierIDs(data.colorModifications);
        }

        public static AvatarData GetAvatarData() {
            AvatarData data = new AvatarData();
            data.featureModifications = SingleModifierID.GetSingleModifierIDs(modificationsOnCharacter);
         //   data.colorModifications = SingleModifierID.GetSingleModifierIDs(colorModification);
            return data;
        }

        public void Push() {
            Push(()=> { 
                Debug.Log("Successfully Pushed");
                DOnFinishPush.Invoke();
            });
            
        }

        public void Push(Action OnPush)
        {
            Debug.Log("Pushing Initiated");
            //update gender
            memService.PushData<int>(UserInfo.Instance.UserId, avAvatarKeys.Gender, gender).OnCompleted((x) =>{
                //update avatar modifier
                string id = UserInfo.Instance.UserId;
                string json = avDictionarySerializer.SerializeDictionary<FeatureGroup, string>(modificationsOnCharacter);
                memService.PushData<string>(id, avAvatarKeys.Avatar_Data, json).OnCompleted((x) => {
                    string cjson = avDictionarySerializer.SerializeDictionary<FeatureGroup, Color>(colorModifiers);
                    //Update color Data
                    memService.PushData<string>(id, avAvatarKeys.Avatar_Color_Data, cjson).OnCompleted((x)=> {
                        
                        OnPush();
                    });
                    
                });
            });

            

        }

        public void Pull(Action OnPull)
        {
            string id =  UserInfo.Instance.UserId;
            memService.PullData<string>(id, avAvatarKeys.Avatar_Data).OnCompleted(
                (x)=> { OnPull(); }    
            );
        }

        public static void GlobalPull(Action OnPull) {
            string id = UserInfo.Instance.UserId;
            //pull avatar data
            memService.PullData<string>(id, avAvatarKeys.Avatar_Data, (x) =>
            {
                var myModifiers = avDictionarySerializer.DeSerializeDictionary<FeatureGroup, string>(x);
                modificationsOnCharacter = myModifiers;

                //pull gender data
                memService.PullData<int>(id, avAvatarKeys.Gender,(x)=> {
                    gender = x;

                    //pull color modification data
                    memService.PullData<string>(id, avAvatarKeys.Avatar_Color_Data,(x)=> {
                        colorModifiers = avDictionarySerializer.DeSerializeDictionary<FeatureGroup, Color>(x);
                        OnPull();
                    });
                    

                });
            });
        }
    }

    [System.Serializable]
    public class SingleModifierID{
        public FeatureGroup group;
        public string modiferId;

        public bool TryGetModifier(out IAvatarModifier modifier) {
            return avGlobalModifierManager.modifiers.TryGetValue(modiferId,out modifier);
        }

        public static List<SingleModifierID> GetSingleModifierIDs(Dictionary<FeatureGroup, string> dic)
        {
            List<SingleModifierID> rs = new List<SingleModifierID>();
            foreach (FeatureGroup key in dic.Keys)
            {
                rs.Add(
                    new SingleModifierID()
                    {
                        group = key,
                        modiferId = dic[key]
                    }
                );
            }

            return rs;
        }

        public static Dictionary<FeatureGroup, string> GetSingleModifierIDs(List<SingleModifierID> lis)
        {
            Dictionary<FeatureGroup, string> rs = new Dictionary<FeatureGroup, string>();
            foreach (var id in lis)
            {
                rs[id.group] = id.modiferId;
            }
            return rs;
        }
    }

    [System.Serializable]
    public class AvatarData {
        public List<SingleModifierID> featureModifications;
        public List<SingleModifierID> colorModifications;

        
    }

    [System.Serializable]
    public class ModifierTable {
        //public List<> modifierId = new List<string>();
        public List<(string, avBlendShapeModifier)> blendShapeModifiers = new List<(string, avBlendShapeModifier)>();

        public ModifierTable(Dictionary<string, IAvatarModifier> modifiersPair) {
            foreach (var item in modifiersPair)
            {
                
            }
        }

        public Dictionary<string, IAvatarModifier> GetDic() {
            return default;
        }   
    }
}