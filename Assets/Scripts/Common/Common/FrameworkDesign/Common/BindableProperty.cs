using System;
namespace LGUVirtualOffice.Framework
{
    public class BindableProperty<T> where T:IEquatable<T>
    {
        private Action<T> mOnValueChanged;
        private T mValue = default(T);
        public T Value 
        {
            get => mValue;
            set 
            {
                if (mValue == null)
                {
                    if (value != null)
                    {
                        mValue = value;
                        Trigger();
                    }
                }
                else if (!mValue.Equals(value))
                {
                    mValue = value;
                    Trigger();
                }
            }
        }
        public void Subscribe(Action<T> onValueChanged) 
        {
            mOnValueChanged += onValueChanged;
        }
        public void UnSubscribe(Action<T> onValueChanged) 
        {
            mOnValueChanged -= onValueChanged;
        }
        public void Clear() 
        {
            mOnValueChanged = null;
        }
        private void Trigger() 
        {
            mOnValueChanged?.Invoke(mValue);
        }
    }
}

