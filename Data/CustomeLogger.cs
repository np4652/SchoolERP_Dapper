
namespace Data
{
    public class CustomeLogger : ICustomeLogger
    {
        protected readonly IDapperRepository _dapperRepository;
        protected readonly string className = "";
        public CustomeLogger(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }
        public async Task<bool> SaveAPILog(APILogRequest request)
        {
            bool res = false;
            string sqlQuery = @"insert into APILog (Request,Response,Method,EntryOn,TID,IsIncomingOutgoing,CallingFrom) 
                                            Values (@request,@response,@method,getDate(),@tid,@IsIncmOut,@CallingFrom)";
            int i = await _dapperRepository.ExecuteAsync(sqlQuery, new
            {
                Request = request.Request ?? string.Empty,
                Response = request.Response ?? string.Empty,
                Method = request.Method ?? string.Empty,
                TID = request.TID ?? string.Empty,
                request.IsIncoming,
                CallingFrom = request.CallingFrom ?? string.Empty
            }, System.Data.CommandType.Text);
            if (i > 0)
            {
                res = true;
            }
            return res;
        }

        public void LogError(Exception ex, string msg = "")
        {
            _dapperRepository.saveLog(new LogRequest
            {
                Exception= ex.ToString(),
                Level= "Error",
                Logger= className,
                Msg= msg,
                Trace="Error",
                URL = string.Empty
            });
        }
    }
    public class CustomeLogger<T> : ICustomeLogger<T>, IDisposable
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly string className = "";
        public CustomeLogger(IDapperRepository dapperRepository)
        {
            className = typeof(T).Name;
            _dapperRepository = dapperRepository;
        }

        public void LogInfo(string msg)
        {
            _dapperRepository.saveLog(new LogRequest
            {
                Exception = string.Empty,
                Level= "Info",
                Logger= className,
                Msg= msg,
                Trace="Info",
                URL = string.Empty
            });
        }

        public void LogWarning(string msg)
        {
            _dapperRepository.saveLog(new LogRequest
            {
                Exception = string.Empty,
                Level= "Warning",
                Logger= className,
                Msg= msg,
                Trace="Warning",
                URL = string.Empty
            });
        }

        public void LogDebug(string msg)
        {
            _dapperRepository.saveLog(new LogRequest
            {
                Exception = string.Empty,
                Level= "Debug",
                Logger= className,
                Msg= msg,
                Trace="Debug",
                URL = string.Empty
            });
        }

        public void LogError(Exception ex,string msg="")
        {
            _dapperRepository.saveLog(new LogRequest
            {
                Exception= ex.ToString(),
                Level= "Error",
                Logger= className,
                Msg= msg,
                Trace="Error",
                URL = string.Empty
            });
        }


        public async Task<bool> SaveAPILog(APILogRequest request)
        {
            bool res = false;
            string sqlQuery = @"insert into APILog (Request,Response,Method,EntryOn,TID,IsIncomingOutgoing,CallingFrom) 
                                            Values (@request,@response,@method,getDate(),@tid,@IsIncmOut,@CallingFrom)";
            int i = await _dapperRepository.ExecuteAsync(sqlQuery, new
            {
                Request = request.Request ?? string.Empty,
                Response = request.Response ?? string.Empty,
                Method = request.Method ?? string.Empty,
                TID = request.TID ?? string.Empty,
                request.IsIncoming,
                CallingFrom = request.CallingFrom ?? string.Empty
            }, System.Data.CommandType.Text);
            if (i > 0)
            {
                res = true;
            }
            return res;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
