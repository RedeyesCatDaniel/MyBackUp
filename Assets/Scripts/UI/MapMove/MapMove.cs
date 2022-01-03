using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LGUVirtualOffice.Framework;
using Newtonsoft.Json;
namespace LGUVirtualOffice
{
    public class MapMove : AbstractController
    {
       


        public static MapMove Instance;
        MapMoveView mapmoveview;
        readonly Dictionary<MapMoveitemview, FavoriteWorkSpaceList.MapmoveTeamData> teamslist = new Dictionary<MapMoveitemview,FavoriteWorkSpaceList.MapmoveTeamData>();
        public MapMoveitemview mapMoveitemview;
        public List<MapMoveitem> itemlist = new List<MapMoveitem>();
        GameObject prefab;
        MapMoveClicked clicked;
        private void Awake()
        {
              Instance = this;
            mapmoveview = GetComponent<MapMoveView>();
            mapmoveview.set.onClick.AddListener(OpenSet);
            mapmoveview.close.onClick.AddListener(()=> 
            {
                gameObject.SetActive(false);
            });
            mapmoveview.search.onValueChanged.AddListener(Search);
        }
        private void OnEnable()
        {
          
            prefab= Resources.Load<GameObject>("UI/Map/item");
            FavoriteWorkSpaceList.Instance.Load(UserInfo.Instance,LoadLocation);
        }
        void Search(string s) 
        {
            foreach (var item in teamslist.Keys)
            {
                if (teamslist[item].teamName.IndexOf(s)>= 0)
                {
                    item.gameObject.SetActive(true);
                }
                else 
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
        public void SetClicked(MapMoveitem placeitem)
        {
            mapMoveitemview.flag.sprite = placeitem.flog[0];
            mapmoveview.Clickitem = placeitem;
        }
        private void LoadLocation(FavoriteWorkSpaceList teams)
        {
            Clear();
            if (teams.mapmoveTeamDatas.Count>0)
            {
                mapMoveitemview.place_name.text = teams.mapmoveTeamDatas[0].teamName;
                mapmoveview.location.text= teams.mapmoveTeamDatas[0].teamName;
                for (int i = 1; i < teams.mapmoveTeamDatas.Count; i++)
                {
                    
                    MapMoveitemview temp = Instantiate(prefab, mapmoveview.item_p).GetComponent<MapMoveitemview>();
                    MapMoveitem moveitem = temp.GetComponent<MapMoveitem>();
                    temp.place_name.text = teams.mapmoveTeamDatas[i].teamName;
                    moveitem.Setflog(teams.mapmoveTeamDatas[i].flogtype);
                    mapmoveview.mapMoveitemviews.Add(temp);
                    teamslist.Add(temp, teams.mapmoveTeamDatas[i]);
                    moveitem.SetImage(1);
                    itemlist.Add(moveitem);
                }; 
            }
        }
        void Clear() 
        {
            mapMoveitemview.flag.sprite = prefab.GetComponent<MapMoveitem>().flog[2];
            foreach (var item in itemlist)
            {
                Destroy(item.gameObject);
            }
            itemlist.Clear();
            mapmoveview.mapMoveitemviews.Clear();
        }
        private void OpenSet()
        {
            //LogUtil.LogInfo("select:" + selectteam.teamName);
            if (clicked == null)
            {
                GameObject prefab_clicked = Resources.Load<GameObject>("UI/Map/Select");
                clicked = Instantiate(prefab_clicked, transform.parent).GetComponent<MapMoveClicked>();
            }
            clicked.gameObject.SetActive(true);
        }
        public void SetFlog(Flogtype flogtype,MapMoveitemview mapMoveitemview) 
        {
            teamslist[mapMoveitemview].flogtype=flogtype;
        }
    }
}