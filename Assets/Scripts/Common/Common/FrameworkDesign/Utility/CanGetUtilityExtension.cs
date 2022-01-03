namespace LGUVirtualOffice.Framework 
{
    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self) where T:class,IUtility 
        {
            return self.GetArchitecture().GetUtility<T>();
        }
    }
}
