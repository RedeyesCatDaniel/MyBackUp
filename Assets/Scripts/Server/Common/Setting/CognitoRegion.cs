using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
namespace LGUVirtualOffice {
    [CreateAssetMenu(fileName = "CognitoRegion", menuName = "ScriptableObjects/CognitoRegion", order = 1)]
    public class CognitoRegion : ScriptableObject
    {
        public Dictionary<string,string> regionToUserPoolIdMapper = new Dictionary<string, string>() {
            { RegionEndpoint.APNortheast2.DisplayName, "ap-northeast-2_cXeGRmfe4"}
        };
        public Dictionary<string, string> regionToIdentityPoolIdMapper = new Dictionary<string, string>() {
            { RegionEndpoint.APNortheast2.DisplayName, "ap-northeast-2:6cf84a4d-86a1-4473-a877-feec4fa9c2c3" }
        };
        public Dictionary<string, string> userPoolIdToClientIdMapper = new Dictionary<string, string>() {
            { "ap-northeast-2_cXeGRmfe4", "1uqmf4bftmbjooeh55r7d74oul"}
        };

        public Dictionary<string, List<string>> userPoolIdToAttributesMapper = new Dictionary<string, List<string>>()
        {
            { "ap-northeast-2_cXeGRmfe4",new List<string>(){"gender","name"} }
        };
    }
}
