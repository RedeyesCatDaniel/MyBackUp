namespace LGUVirtualOffice.Framework 
{
    public static class CanGetServiceExtension
    {
        public static T GetService<T>(this ICanGetService self) where T : class, IService 
        {
            return self.GetArchitecture().GetService<T>();
        }
    }
}
