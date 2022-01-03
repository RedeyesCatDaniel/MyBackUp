using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
namespace LGUVirtualOffice
{ 
    public enum Flogtype 
        {
            flog,star,floged,stared
        }
    public class MapMoveitem : AbstractController
    {
       
        MapMoveitemview placeitemview;
        public List<Sprite> clickde;
        public List<Sprite> flog;
        Flogtype flogtype;
        void Awake()
        {
            placeitemview = GetComponent<MapMoveitemview>();
            placeitemview.cliked.onClick.AddListener(() =>
            {
                MapMove.Instance.SetClicked(this);
            });
            placeitemview.flog_btn.onClick.AddListener(()=> 
            {
                flogtype = (Flogtype)(((int)flogtype + 2) % 4);
                MapMove.Instance.SetFlog(flogtype, placeitemview);
                Setflog(flogtype);
            });
           
        }
        public void Setflog(Flogtype flogtype) 
        {
            this.flogtype = flogtype;
            switch (flogtype)
            {
                case Flogtype.flog:
                    placeitemview.flag.sprite = flog[0];
                    break;
                case Flogtype.star:
                    placeitemview.flag.sprite = flog[1];
                    break;
                case Flogtype.floged:
                    placeitemview.flag.sprite = flog[2];
                    break;
                case Flogtype.stared:
                    placeitemview.flag.sprite = flog[3];
                    break;
                default:
                    break;
            }

        }
       
        public void SetImage(int index)
        {
            try
            {
                placeitemview.oncliked.sprite = clickde[index];
            }
            catch (Exception e)
            {
                Exception ex = new Exception("Sprite index errow\n"+e.Message); 
                LogUtil.LogException(ex);
            }
        }
        public void Select()
        {
            placeitemview.background.color = new Color(238f/255f,241f/255f,1,1);
            SetImage(0);
        }
        public void NoSelect()
        {
            placeitemview.background.color = Color.white;
            SetImage(1);
        }
    }
}