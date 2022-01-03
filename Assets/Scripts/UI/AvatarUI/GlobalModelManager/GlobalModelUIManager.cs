using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class GlobalModelUIManager : MonoBehaviour
    {

        public UnityEvent<int> DOnGroupSelected;
       // public UnityEvent<int> DOnAwake;
        public static GlobalModelUIManager instance;

        public avAvatarPlaceHolderManager[] myBodies;
        public avAvatarFeatureOptionManager[] FeatureOptions;
        
       // public List<ListStruct<avFeatureOptions>> options;
       // public List<ListStruct<Color>> colors;
        public List<Sprite> myGroup;
        public List<FeatureGroup> fGroup;
        public List<int> rowCount;
        //public List<string> titles;

        
         
        private int CurrentGroup;
        public avPageManager avpman;
        public avAvatarDataApplier defaultData;
        

        private void Awake()
        {
            instance = this;
            
            
        }

        private void Start()
        {
            avAvatarRenderer renderer = myBodies[avGlobalModifierManager.gender].renderer;
            avGlobalModifierManager.Modify(renderer);

            for (int i = 0; i < myBodies.Length; i++)
            {
                if (i != avGlobalModifierManager.gender)
                {
                    defaultData.Apply(i);
                }
                else {
                    if (avGlobalModifierManager.modificationsOnCharacter.Count == 0) {
                        defaultData.Apply(i);
                    }
                }
            }
        }

        public void ReFresh() {
            Debug.Log("I refreshed page");
            DOnGroupSelected.Invoke(CurrentGroup);
        }

        public List<Sprite> GetGroupICons()
        {
            List<Sprite> rs = new List<Sprite>();
            foreach (var tu in myGroup)
            {
                rs.Add(tu);
            }

            return rs;
        }
        
        //public int GetFeatureGroupCount (int index){
        //    if (index < options.Count && index >= 0)
        //    {
        //        return options[index].Count;
        //    }
        //    else {
        //        return 0;
        //    }      
        //}

        public int GetFeatureGroupCount(int index)
        {
            FeatureGroup featureGroup = fGroup[index];
            int genderindex = avGlobalModifierManager.gender;
            return FeatureOptions[genderindex].GetFeatureCount(featureGroup);
        }

        

        public void OnGroupSelected(int group) {
            CurrentGroup = group;
            DOnGroupSelected?.Invoke(group);
            //Debug.Log($"Group {group} is selected");
            avpman.ShowRowUtil(rowCount[group]);
        }

        
        //this method will try to get modifier and apply modification to body renderer
        public void OnSingleFeatureSelected(int index) {
            //Debug.Log($"Feature {index} is selected");
            avAvatarFeatureOptionManager om = FeatureOptions[avGlobalModifierManager.gender];
            //avFeatureOptions option = options[CurrentGroup][index];
            string id = om.GetID(GetCurrentGroup(),index);
            ApplyModification(avGlobalModifierManager.gender, fGroup[CurrentGroup], id);
            //if (avGlobalModifierManager.TryGetModifier(id, out IAvatarModifier modifier)) {
            //    avGlobalModifierManager.modificationsOnCharacter[fGroup[CurrentGroup]] = id;               
            //    modifier.Modify(myBodies[avGlobalModifierManager.gender]);
            //}

        }

        public void ApplyModification(int gender,FeatureGroup group, string modifierName) {

            if (avGlobalModifierManager.TryGetModifier(modifierName, out IAvatarModifier modifier))
            {
                avGlobalModifierManager.modificationsOnCharacter[group] = modifierName;
                modifier.Modify(myBodies[gender].renderer);
            }
        }

        //public Sprite GetFeatureSprite(int index) {
        //    if (index < options.Count && index >=0) {
        //        ListStruct<avFeatureOptions> lis = options[CurrentGroup];
        //        Debug.Log($"I am here and Index is {index}");
        //        if (lis.TryGet(index,out avFeatureOptions value)) {
        //            return value.sprite;
        //        }
                
        //    }
        //    return null;
            
        //}

        public Sprite GetFeatureSprite(int index)
        {
            FeatureGroup featureGroup = GetCurrentGroup();
            int genderindex = avGlobalModifierManager.gender;
            return FeatureOptions[genderindex].GetSprite(featureGroup,index);
        }

        public FeatureGroup GetCurrentGroup() {
            if (CurrentGroup < fGroup.Count) {
                return fGroup[CurrentGroup];
            }
            return default;
        }

        //public List<Color> GetCurrentColors() {
        //    if (CurrentGroup < colors.Count)
        //    {
        //        return colors[CurrentGroup].Values;
        //    }
        //    return new List<Color>();
        //}

        public List<Color> GetCurrentColorsModifier()
        {
            FeatureGroup group = fGroup[CurrentGroup];
            if (avGlobalModifierManager.TryGetInstance(out avGlobalModifierManager instance)) {
                if (instance.GetColorByFeatureGroup().TryGetValue(group,out List<Color> value)) {
                    return value;
                }
            }
            return new List<Color>();
        }

        public void OnChangeColorTo(int index) {
            FeatureGroup group = fGroup[CurrentGroup];
            Color c = GetCurrentColorsModifier()[index];
            ChangeColor(avGlobalModifierManager.gender, group,c);
            //avGlobalModifierManager.colorModifiers[group] = c;

            //avAvatarRenderer renderer = myBodies[avGlobalModifierManager.gender];
            //renderer.ChangeColor(group,c);
            //  Debug.Log($"I have changed color to {index}");
        }

        public void ChangeColor(int gender,FeatureGroup group, Color color) {
            avGlobalModifierManager.colorModifiers[group] = color;
            avAvatarRenderer renderer = myBodies[gender].renderer;
            renderer.ChangeColor(group, color);
        }

        //public string GetCurrentTitle(int index) {
        //    return titles[index];
        //}

        //public IAvatarModifier GetModifier(int index) { 
        
        //}

    }

    [System.Serializable]
    public class ListStruct<T>{
        [SerializeField]
        private List<T> values = new List<T>();
        public List<T> Values => values;
        public int Count => values.Count;
        public T this[int index] { 
            get => values[index];
        }

        public bool TryGet(int index, out T value) {


            if (values.Count > index && index >= 0)
            {
                Debug.Log($"I find {index} in a {values.Count} sized {typeof(T)} array");
                value = values[index];
                return true;
            }
            else {
                Debug.Log($"I cannot find {index} in a {values.Count} sized {typeof(T)} array");
                value = default;
                return false;
            }
        }

        
    }

    

   
}