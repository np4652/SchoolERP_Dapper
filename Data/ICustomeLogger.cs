

namespace Data
{
    public interface ICustomeLogger
    {
        Task<bool> SaveAPILog(APILogRequest request);
        void LogError(Exception error, string message = "");
    }
    public interface ICustomeLogger<T> : ICustomeLogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogDebug(string message);
    }
}
