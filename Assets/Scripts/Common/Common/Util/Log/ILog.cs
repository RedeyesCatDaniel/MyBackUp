using System;
namespace LGUVirtualOffice.Log
{
    public interface ILog { 
        void LogDebug(object debugMessage);
        void LogException(Exception e, string message);
        void  Log(object message);
        void  Log<T>(T message)where T:JsonData;
        void LogException(Exception e);
        void LogError(object message);
        void LogWarning(object message);
        void Assert(bool condition, object message);
    }
}