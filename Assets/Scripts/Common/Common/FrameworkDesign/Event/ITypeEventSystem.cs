using System;
namespace LGUVirtualOffice.Framework 
{
    public interface ITypeEventSystem
    {
        void Trigger<T>() where T : IEvent,new();
        void Trigger<T>(T e) where T:IEvent;
        IUnSubscribe Subscribe<T>(Action<T> mOnEvent) where T:IEvent;
        void UnSubscribe<T>(Action<T> mOnEvent) where T : IEvent;
    }
}
