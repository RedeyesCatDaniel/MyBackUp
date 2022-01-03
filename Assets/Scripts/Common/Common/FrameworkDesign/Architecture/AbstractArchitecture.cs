using System;
using UnityEngine;
using System.Collections.Generic;
namespace LGUVirtualOffice.Framework 
{
    public abstract class AbstractArchitecture<T> :IArchitecture where T : AbstractArchitecture<T>, new()
    {
        IOCContainer Container { get; set; }
        Dictionary<Type, ICommand> commandPool;
        private static T mArchitecture;
        private ITypeEventSystem typeEventSystem;
        private static bool mInited = false;
        private List<IModel> modelList=new List<IModel>();
        private List<IService> serviceList = new List<IService>();
        protected abstract void OnInit();

        private static Action<T> mOnRegisterPatch;

        public static void SubscribeRegisterpatch(Action<T> onRegisterPatch,bool triggerNow=false) 
        {
            if (triggerNow)
            {
                CreateArchitecture();
                onRegisterPatch.Invoke(mArchitecture);
            }
            else 
            {
                mOnRegisterPatch += onRegisterPatch;
            }
        }
        private static void CreateArchitecture() 
        {
            if (mArchitecture == null)
            {
                mArchitecture = new T();
                mArchitecture.Container = new IOCContainer();
                mArchitecture.commandPool = new Dictionary<Type, ICommand>();
                mArchitecture.typeEventSystem = new TypeEventSystem();
            }
        }
        private static void InitArchitecture() 
        {
            if (mArchitecture == null) 
            {
                CreateArchitecture();
            }
            if (!mInited) 
            {
                mArchitecture.OnInit();
                mInited = true;
                mOnRegisterPatch?.Invoke(mArchitecture);
                mOnRegisterPatch = null;
                InitModel();
                InitService();
            }
        }
        private static void InitModel() 
        {
            foreach (var item in mArchitecture.modelList)
            {
                item.Init();
            }
            mArchitecture.modelList.Clear();
        }
        private static void InitService() 
        {
            foreach (var item in mArchitecture.serviceList)
            {
                item.Init();
            }
            mArchitecture.serviceList.Clear();
        }
        public K GetService<K>() where K : class, IService 
        {
            return Get<K>();
        }
        public K GetUtility<K>() where K : class,IUtility
        {
            return Get<K>();
        }
        public K GetModel<K>() where K : class,IModel 
        {
            return Get<K>();
        }
        public void RegisterUtility<K>(K utilityInstance) where K: class,IUtility
        {
            Register<K>(utilityInstance);
        }
        public void RegisterModel<K>(K modelInstance) where K : class, IModel 
        {
            modelInstance.SetArchitecture(this);
            Register<K>(modelInstance);
            if (mInited)
            {
                modelInstance.Init();
            }
            else 
            {
                modelList.Add(modelInstance);
            }
        }

        public void RegisterService<K>(K serviceInstance) where K : class, IService 
        {
            serviceInstance.SetArchitecture(this);
            Register<K>(serviceInstance);
            if (mInited)
            {
                serviceInstance.Init();
            }
            else
            {
                serviceList.Add(serviceInstance);
            }
        }

        public void SendCommand<K>() where K : ICommand, new() 
        {
            Type t = typeof(K);
            if (!commandPool.ContainsKey(t))
            {
                K command = new K();
                command.SetArchitecture(this);
                commandPool.Add(t, command);
                command.Excute();   
            }
            else 
            {
                commandPool[t].Excute();
            }
        }

        public void SendCommand<K>(K command) where K : ICommand
        {
            command.SetArchitecture(this);
            command.Excute();
        }

        public void TriggerEvent<K>() where K : IEvent, new() 
        {
            typeEventSystem.Trigger<K>();
        }
        public void TriggerEvent<K>(K e) where K : IEvent 
        {
            typeEventSystem.Trigger<K>(e);
        }
        public IUnSubscribe SubscribeEvent<K>(Action<K> mOnEvent) where K : IEvent
        {
            return typeEventSystem.Subscribe<K>(mOnEvent);
        }
        public void UnSubscribeEvent<K>(Action<K> mOnEvent) where K : IEvent 
        {
            typeEventSystem.UnSubscribe<K>(mOnEvent);
        }
        private void Register<K>(K objInstance) where K : class
        {
            CreateArchitecture();
            Container.Register<K>(objInstance);
        }
        private K Get<K>() where K : class 
        {
            InitArchitecture();
            return Container.Get<K>();
        }
    }
}
