using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice
{
    public class FavoriteWorkSpaceList : Singleton<FavoriteWorkSpaceList>
    {
        private FavoriteWorkSpaceList(){}
        //readonly string key = "FavoriteWorkSpaceList";
        public List<MapmoveTeamData> mapmoveTeamDatas = new List<MapmoveTeamData>();
        public int index;
        public class MapmoveTeamData:IComparer<MapmoveTeamData>
        {
            public string teamName;
            public Flogtype flogtype;

            public int Compare(MapmoveTeamData x, MapmoveTeamData y)
            {
                if (x.flogtype!=y.flogtype)
                {
                    return x.flogtype - y.flogtype;
                }
                return x.teamName.CompareTo(y.teamName);
            }

            
        }
        public void Load(UserInfo userInfo,Action<FavoriteWorkSpaceList> action)
        {
            //memService.PullData<string>(userInfo.UserId, key, (teamdata) =>
            //     {

            //         FavoriteWorkSpaceList data = JsonConvert.DeserializeObject<FavoriteWorkSpaceList>(teamdata);
            //         Instance.index = data.index;
            //         Instance.mapmoveTeamDatas = data.mapmoveTeamDatas;
            //         action.Invoke(Instance);
            //     });
            string json = userInfo.FavoriteWorkSpaceList;

            if(json!=null)
            {
                FavoriteWorkSpaceList data = JsonConvert.DeserializeObject<FavoriteWorkSpaceList>(json);
                Instance.index = data.index;
                Instance.mapmoveTeamDatas = data.mapmoveTeamDatas;
                MapmoveTeamData temp = Instance.mapmoveTeamDatas[index];
                Instance.mapmoveTeamDatas[index] = Instance.mapmoveTeamDatas[0];
                Instance.mapmoveTeamDatas[0] = temp;
                Instance.index = 0;
                Instance.mapmoveTeamDatas.Sort(1, Instance.mapmoveTeamDatas.Count - 1, temp);
                //foreach (var item in data.mapmoveTeamDatas)
                //{
                //    Instance.mapmoveTeamDatas.Add(item);
                //}
            }
            else
            {
                Instance.mapmoveTeamDatas = new List<MapmoveTeamData>();
                LogUtil.LogError("no Data\n"+JsonConvert.SerializeObject(userInfo));
            }
            action.Invoke(Instance);
        }
        public void Updata(UserInfo userInfo) 
        {
            //Instance.mapmoveTeamDatas = new List<MapmoveTeamData>();
            //Instance.mapmoveTeamDatas.Add(new MapmoveTeamData() { teamName = "언택트서비스TF2", flogtype = Flogtype.flog });
            //Instance.mapmoveTeamDatas.Add(new MapmoveTeamData() { teamName = "차세대기술발굴2팀2", flogtype = Flogtype.floged });
            ////mapMoveTeamList.index = 0;

            string json = JsonConvert.SerializeObject(Instance);
            //LogUtil.LogInfo(json);
            userInfo.FavoriteWorkSpaceList = json;// JsonConvert.SerializeObject(Instance);
            //memService.PushData<string>(UserInfo.Instance.UserId, key, JsonConvert.SerializeObject(Instance), (x) =>
            //{
            //    if (x)
            //    {
            //        LogUtil.LogInfo("Push success");
            //    };
            //});
        }

       
    }
}

