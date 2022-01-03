using System;
namespace LGUVirtualOffice.Framework 
{
    public static class CanSubscribeEventExtension
    {
        public static IUnSubscribe SubscribeEvent<T>(this ICanSubscribeEvent self, Action<T> mOnEvent) where T:IEvent
        {
           return self.GetArchitecture().SubscribeEvent<T>(mOnEvent);
        }
        public static void UnSubscribeEvent<T>(this ICanSubscribeEvent self, Action<T> mOnEvent) where T : IEvent 
        {
            self.GetArchitecture().UnSubscribeEvent<T>(mOnEvent);
        }
    }
}
