using Dapper;
using Data;
using Entities.Enums;
using Entities;
using System.Data;
using Infrastructure.Interface;

namespace Service
{
    public class PGDetailService : IPGDetailService
    {
        private IDapperRepository _dapper;
        public PGDetailService(IDapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IResponse> AddAsync(PaymentGatewayModel paymentGateway)
        {
            Response res = new Response();
            try
            {
                var result = await _dapper.ExecuteAsync("proc_addpaymentgateway", new
                {
                    paymentGateway.Id,
                    paymentGateway.PGId,
                    paymentGateway.BaseURL,
                    paymentGateway.MerchantID,
                    paymentGateway.MerchantKey,
                    paymentGateway.SuccessURL,
                    paymentGateway.FailURL,
                    paymentGateway.StatusCheckURL,
                    paymentGateway.IsLive,
                    paymentGateway.IsLoggingTrue,
                    paymentGateway.IsActive,

                }, commandType: CommandType.StoredProcedure);

                return new Response
                {
                    StatusCode = result != -1 ? ResponseStatus.Success : ResponseStatus.Failed,
                    ResponseText = result != -1 ? ResponseStatus.Success.ToString() : ResponseStatus.Failed.ToString(),
                };
            }
            catch (Exception ex)
            {
                res = new Response
                {
                    StatusCode = ResponseStatus.Failed,
                    ResponseText = ResponseStatus.Failed.ToString(),
                };
            }
            return res;
        }

        public async Task<IEnumerable<PaymentGatewayModel>> GetAllAsync(int loginId = 0,PaymentGatewayModel paymentGateway = null)
        {
            string sqlQuery = @"Select * from PaymentGatwaydetails(nolock)";
            paymentGateway = paymentGateway == null ? new PaymentGatewayModel() : paymentGateway;
            var res = await _dapper.GetAllAsync<PaymentGatewayModel>(paymentGateway, sqlQuery);
            return res ?? new List<PaymentGatewayModel>();
        }

        public async Task<IResponse<PaymentGatewayModel>> GetByIdAsync(int id)
        {
            var response = new Response<PaymentGatewayModel>
            {
                StatusCode = ResponseStatus.Failed
            };
            var dbparams = new DynamicParameters();
            dbparams.Add("ID", id);
            string sqlQuery = @"Select * from PaymentGatwaydetails(nolock) where ID=@Id";
            var result = await _dapper.GetAsync<PaymentGatewayModel>(sqlQuery, dbparams, commandType: CommandType.Text);
            if (result != null)
            {
                response = new Response<PaymentGatewayModel>
                {
                    StatusCode = ResponseStatus.Success,
                    Result = result
                };
            }
            return response;
        }



        public Task<IResponse> DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IReadOnlyList<Entities.PaymentGatewayModel>> GetDropdownAsync(Entities.PaymentGatewayModel entity)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Response> ChangeLiveStatus(int id)
        {
            var response = new Response()
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Failed to update"
            };
            try
            {
                string sqlQuery = @"UPDATE PaymentGatwaydetails SET IsLive = 1^IsLive Where id = @id";
                int i = await _dapper.ExecuteAsync(sqlQuery, new { id }, CommandType.Text);
                if (i > -1)
                {
                    response.StatusCode = ResponseStatus.Success;
                    response.ResponseText = "Updated Successfully";
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<Response> ChangeLoggingStatus(int id)
        {
            string sqlQuery = @"UPDATE PaymentGatwaydetails SET IsLoggingTrue = 1^IsLoggingTrue Where id = @Id";
            int i = await _dapper.ExecuteAsync(sqlQuery, new { id }, CommandType.Text);
            var response = new Response();
            if (i > -1)
            {
                response.StatusCode = ResponseStatus.Success;
                response.ResponseText = ResponseStatus.Success.ToString();
            }
            return response;
        }


        public async Task<Response> ChangeActiveStatus(int id)
        {

            string sqlQuery = @"UPDATE PaymentGatwaydetails SET IsActive = 1^IsActive Where Id = @id";
            int i = await _dapper.ExecuteAsync(sqlQuery, new { id }, CommandType.Text);
            var response = new Response();
            if (i > -1)
            {
                response.StatusCode = ResponseStatus.Success;
                response.ResponseText = ResponseStatus.Success.ToString();
            }
            return response;
        }

        public async Task<Response<PaymentGatewayModel>> GetPGDetailByPGIDAsync(PaymentGatewayType paymentGatewayType)
        {
            var response = new Response<PaymentGatewayModel>
            {
                StatusCode = ResponseStatus.Failed
            };
            var dbparams = new DynamicParameters();
            dbparams.Add("pgId", paymentGatewayType);
            string sqlQuery = @"Select *, Other VPA from PaymentGatwaydetails(nolock) where PGId=@pgId";
            var result = await _dapper.GetAsync<PaymentGatewayModel>(sqlQuery, dbparams, commandType: CommandType.Text);
            if (result != null)
            {
                response = new Response<PaymentGatewayModel>
                {
                    StatusCode = ResponseStatus.Success,
                    Result = result
                };
            }
            return response;
        }

        public Task<IEnumerable<Response>> UpdateTransactionStatus(string Status, int TID, string UTR = "")
        {
            string sp = "proc_UpdateStatus";
            var res = _dapper.GetAllAsync<Response>(sp, new { Status = Status[0], TID, UTR }, CommandType.StoredProcedure);
            return res;
        }
    }
}
