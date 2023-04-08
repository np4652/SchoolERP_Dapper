using Data;
using Entities.Models;
using static Service.PaymentGateway.CashFree.Models;

namespace Service.PaymentGateway
{
    public interface IPaymentGatewayService
    {
        Task<Response<PaymentGatewayResponse>> GeneratePGRequestForWebAsync(PaymentGatewayRequest request);
        Task<Response<CashFreeResponseForApp>> GeneratePGRequestForAppAsync(PaymentGatewayRequest request);
        Task<Response<int>> SaveInitiatePayment(PaymentGatewayRequest request, int packageId);
        Task<Response<StatusCheckResponse>> StatusCheck(StatusCheckRequest request);
        Task<Response<PaymentGatewayRequest>> GetInitiatedPaymentDetail(int TID);
        Task<Response> updateInitiatedPayment(int TID, string status);
        Task APILog(string url, string request, string response);
    }
}