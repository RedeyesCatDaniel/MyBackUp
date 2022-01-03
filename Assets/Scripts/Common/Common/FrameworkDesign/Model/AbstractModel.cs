namespace LGUVirtualOffice.Framework 
{
    public abstract class AbstractModel : IModel
    {
        private IArchitecture architecture;
        IArchitecture ICanGetArchitecture.GetArchitecture()
        {
            return architecture;
        }

        void IModel.Init()
        {
            OnInit();
        }
        protected abstract void OnInit();
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architecture = architecture;
        }
    }
}
