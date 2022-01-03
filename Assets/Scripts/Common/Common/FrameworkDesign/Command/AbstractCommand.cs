namespace LGUVirtualOffice.Framework 
{
    public abstract class AbstractCommand : ICommand
    {
        private IArchitecture architecture;
        void ICommand.Excute()
        {
            OnExcute();
        }
        protected abstract void OnExcute();
        IArchitecture ICanGetArchitecture.GetArchitecture()
        {
            return architecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architecture=architecture;
        }
    }
}
