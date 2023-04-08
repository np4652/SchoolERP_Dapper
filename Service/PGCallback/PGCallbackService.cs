using AutoMapper;
using Data;
using Entities.Enums;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Service.PaymentGateway.PayU;
using Service.Models;
using System.Data;

namespace Service.CartWishList
{
    public interface IPGCallback
    {
        Task<Response> PayUnotify(RequestBase<PayUResponse> request);
    }
    public class PGCallbackService : IPGCallback
    {
        private IDapperRepository _dapper;
        private readonly ICustomeLogger _logger;
        private readonly IMapper _mapper;
        public PGCallbackService(IDapperRepository dapper, ICustomeLogger logger, IMapper mapper)
        {
            _mapper = mapper;
            _dapper = dapper;
            _logger = logger;
        }
        public async Task<Response> PayUnotify(RequestBase<PayUResponse> request)
        {
            string sp = "proc_pgCallBackUpdate";
            var res = new Response();
            try
            {
                PayUService p = new PayUService(_logger, _dapper, _mapper);
                var req = new StatusCheckRequest()
                {
                    PGID = PaymentGatewayType.PayU,
                    TID = Convert.ToInt32(request.Data.txnid.Replace("TID", ""))
                };
                var statusCheck = await p.StatusCheckPG(req);
                res = await _dapper.GetAsync<Response>(sp, new
                {
                    OrderId= statusCheck.Result.OrderId,
                    OrderStatus=statusCheck.Result.OrderStatus,
                    OrderAmount=statusCheck.Result.OrderAmount,
                    ReferenceId=statusCheck.Result.ReferenceId,
                },CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return res;
        }
       
    }
}
