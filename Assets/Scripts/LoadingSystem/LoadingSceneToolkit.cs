using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "Toolkit/LoadingSceneToolkit")]
    public class LoadingSceneToolkit :ScriptableObject
    {
        private LoadingSceneManager LoadingScene;
        public LoadingSceneManager loadingScene;
        //This Method will turn the loading scene on
        public void Show() {
            if (LoadingScene == null)
            {
                LoadingScene = Instantiate(loadingScene);
                DontDestroyOnLoad(LoadingScene);
            }

            LoadingScene.Progress = 0;
            LoadingScene.DOnFinish.RemoveListener(Finish);
            LoadingScene.DOnFinish.AddListener(Finish);
            LoadingScene.gameObject.SetActive(true);
            
        }

        //this method will turn the loading scene off
        public void Hide() {
            Debug.Log("I tried to hide Scene");
            if (LoadingScene != null)
            {
                
                LoadingScene.gameObject.SetActive(false);
            } 
        }

        public void AddProgress(float percentage) {
            if(LoadingScene!=null)
                LoadingScene.Progress += percentage;
        }

        public void SetProgress(float percentage) {
            if (LoadingScene!=null) {
                LoadingScene.Progress = percentage;
            }
            
        }

        public void Finish() {
            Hide();
        }

        public void Kill() {
            if(LoadingScene!=null)
                Destroy(LoadingScene);
        }
    }
}