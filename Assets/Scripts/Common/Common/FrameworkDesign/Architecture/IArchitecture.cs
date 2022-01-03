using System;
namespace LGUVirtualOffice.Framework 
{ 
    public interface IArchitecture
    {
        void RegisterModel<T>(T modeInstance) where T : class, IModel;
        void RegisterUtility<T>(T utilityInstance) where T : class,IUtility;
        void RegisterService<T>(T serviceInstance) where T : class,IService;
        T GetService<T>() where T : class, IService;
        T GetUtility<T>() where T : class,IUtility;
        T GetModel<T>() where T : class,IModel;

        void SendCommand<T>() where T : ICommand,new();
        void SendCommand<T>(T command) where T : ICommand;
        void TriggerEvent<T>() where T : IEvent, new();
        void TriggerEvent<T>(T e) where T : IEvent;
        IUnSubscribe SubscribeEvent<T>(Action<T> mOnEvent) where T : IEvent;
        void  UnSubscribeEvent<T>(Action<T> mOnEvent) where T : IEvent;
 }
}
