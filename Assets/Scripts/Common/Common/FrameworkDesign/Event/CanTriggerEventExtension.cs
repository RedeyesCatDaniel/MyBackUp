namespace LGUVirtualOffice.Framework 
{
    public static class CanTriggerEventExtension
    {
        public static void TriggerEvent<T>(this ICanTriggerEvent self) where T:IEvent,new()
        {
            self.GetArchitecture().TriggerEvent<T>();
        }
        public static void TriggerEvent<T>(this ICanTriggerEvent self, T e) where T : IEvent 
        {
            self.GetArchitecture().TriggerEvent<T>(e);
        }
    }
}
