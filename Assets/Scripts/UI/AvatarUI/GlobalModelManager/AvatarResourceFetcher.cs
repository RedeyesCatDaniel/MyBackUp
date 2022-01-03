using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public interface IAvatarResourceFetcher {
        List<FeatureSelection> FetchFeationSelections(FeatureGroup group);
        Sprite FetchSelctionSprite(FeatureSelection selection);
    }

    public class AvatarResourceFetcher : MonoBehaviour, IAvatarResourceFetcher
    {
        public Dictionary<FeatureGroup, List<FeatureSelection>> fselections;
        public Dictionary<FeatureSelection, Sprite> featureSprite;
        public List<FeatureSelection> FetchFeationSelections(FeatureGroup group)
        {
            throw new System.NotImplementedException();
        }

        public Sprite FetchSelctionSprite(FeatureSelection selection)
        {
            throw new System.NotImplementedException();
        }
    }
}