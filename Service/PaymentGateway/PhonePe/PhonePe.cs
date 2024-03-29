﻿//using System.Text;
//using Data;
//using Entities.Enums;
//using Entities;
//using Entities.Models;
//using Newtonsoft.Json;
//using AppUtility.APIRequest;
//using AppUtility.Extensions;

//namespace Service.PaymentGateway.PhonePe
//{
//    public class PhonePe : PaymentGatewayBase
//    {
//        private readonly string apiVersion = "2022-09-01";

//        private readonly Dictionary<string, string> paymentModes = new Dictionary<string, string>(){
//            {"CCRD", "cc"},
//            {"DCR", "dc"},
//            {"PWLT", "PPI wallet"},
//            {"NBNK", "nb"},
//            {"UPI", "upi"},
//            {"37UPI", "upi"},
//        };
//        public PhonePe(ICustomeLogger logger, IDapperRepository dapper) : base(logger, dapper)
//        {
//        }
//        public override async Task<Response<PaymentGatewayResponse>> GeneratePGRequestForWebAsync(PaymentGatewayRequest request)
//        {
//            Response<PaymentGatewayResponse> res = new Response<PaymentGatewayResponse>
//            {
//                StatusCode = ResponseStatus.Failed,
//                ResponseText = ResponseStatus.Failed.ToString(),
//                Result = new PaymentGatewayResponse<CashfreeOrderResponse>
//                {
//                    Data = new CashfreeOrderResponse()
//                }
//            };
//            string paymentMode = string.Empty;
//            if (paymentModes.ContainsKey(request.PaymentModeShortName ?? string.Empty))
//            {
//                paymentMode = paymentModes[request.PaymentModeShortName];
//            }
//            var cashfreeRequest = new CashfreeOrderRequest
//            {
//                order_amount = (double)request.Amount,
//                order_currency = "INR",
//                order_id = request.TID,
//                payment_capture = 1,
//                customer_details = new CustomerDetails
//                {
//                    customer_id = request.UserID.ToString(),
//                    customer_email = request.EmailID,
//                    customer_phone = request.MobileNo
//                },
//                order_meta = new OrderMeta
//                {
//                    payment_methods = paymentMode,
//                    return_url = request.Domain + "/CashFreereturn?order_id={order_id}&order_token={order_token}",
//                    notify_url = request.Domain + "/CashFreenotify"//"https://roundpay.net/Callback/45"
//                },
//                order_expiry_time = DateTime.Now.AddMinutes(30).ToString("yyyy-MM-ddTHH:mm:ssZ")
//            };
//            if (apiVersion=="2022-09-01")
//            {
//                cashfreeRequest.order_meta.return_url = request.Domain + "/CashFreereturn?order_id={order_id}";
//            }
//            CashfreeOrderResponse cashfreeResponse = new CashfreeOrderResponse();
//            try
//            {
//                string baseUrl = request.URL;//"https://sandbox.cashfree.com/pg/orders";https://api.cashfree.com/pg/orders
//                string clientId = request.MerchantID;
//                string secretKey = request.MerchantKey;
//                var headers = new Dictionary<string, string>
//                {
//                    {"x-client-id", clientId},
//                    {"x-client-secret", secretKey},
//                    {"x-api-version", apiVersion}
//                };
//                string reponse = await AppWebRequest.O.PostJsonDataUsingHWRTLS(baseUrl, cashfreeRequest, headers).ConfigureAwait(false);
//                cashfreeResponse = JsonConvert.DeserializeObject<CashfreeOrderResponse>(reponse);
//                res.ResponseText = cashfreeResponse.message;
//                if (cashfreeResponse.code?.ToLower().In("request_failed", "order_meta.return_url_invalid", "order_already_exists") ?? false)
//                {
//                    res.ResponseText = cashfreeResponse.message;
//                }
//                else
//                {
//                    res.StatusCode = ResponseStatus.Success;
//                    res.ResponseText = "Transaction intiated";
//                    res.Result = new PaymentGatewayResponse<CashfreeOrderResponse>
//                    {
//                        URL = apiVersion!="2022-09-01" ? cashfreeResponse.payment_link : cashfreeResponse.payments.url,
//                        TID = request.TID,
//                        PGType = PaymentGatewayType.CASHFREE,
//                        Data = cashfreeResponse,
//                        APIResponse = cashfreeResponse,
//                    };
//                }
//            }
//            catch (Exception ex)
//            {
//                string errorMsg = string.Concat(ex.Message, " | request : ", JsonConvert.SerializeObject(cashfreeRequest), " | response : ", JsonConvert.SerializeObject(cashfreeResponse));
//                LogError(ex, errorMsg);
//            }
//            if (request.IsLoggingTrue)
//            {
//                //_apiLogin.SaveLog(JsonConvert.SerializeObject(cashfreeRequest), JsonConvert.SerializeObject(cashfreeResponse), "Cashfree", request.TID);
//            }
//            return res;
//        }

//        public override async Task<Response<StatusCheckResponse>> StatusCheck(StatusCheckRequest request)
//        {
//            Response<StatusCheckResponse> res = new Response<StatusCheckResponse>
//            {
//                StatusCode = ResponseStatus.Error,
//                ResponseText = ResponseStatus.Error.ToString(),
//                Result = new StatusCheckResponse
//                {
//                    OrderStatus=ResponseStatus.Pending.ToString()
//                }
//            };
//            StringBuilder param = new StringBuilder("appId={{appId}}&secretKey={{secretKey}}&orderId={{orderId}}");
//            string cashfreeresp = string.Empty;
//            string orderId = string.Empty;
//            PaymentGatewayModel pgConfig = new PaymentGatewayModel();
//            try
//            {
//                pgConfig = await GetConfiguration(request.PGID);
//                orderId = $"TID{request.TID.ToString().PadLeft(5, '0')}K";
//                param.Replace("{{appId}}", pgConfig.MerchantID);
//                param.Replace("{{secretKey}}", pgConfig.MerchantKey);
//                param.Replace("{{orderId}}", orderId);
//                cashfreeresp = await AppWebRequest.O.CallUsingHttpWebRequest_POSTAsync(pgConfig.StatusCheckURL, param.ToString());
//                if (!string.IsNullOrEmpty(cashfreeresp))
//                {
//                    if (cashfreeresp.Contains("<html>"))
//                    {
//                        goto Finish;
//                    }
//                    var APIResponse = JsonConvert.DeserializeObject<CashfreeStatusResponse>(cashfreeresp);
//                    res.ResponseText = APIResponse.reason;
//                    if (APIResponse != null && (!APIResponse.status?.Equals("ERROR", StringComparison.OrdinalIgnoreCase) ?? false))
//                    {
//                        APIResponse.txStatus = APIResponse.txStatus == null ? String.Empty : APIResponse.txStatus;
//                        res.Result.OrderId = orderId;
//                        res.Result.OrderAmount = APIResponse.orderAmount;
//                        res.StatusCode = ResponseStatus.Success;
//                        res.Result.OrderStatus = !string.IsNullOrEmpty(APIResponse.txStatus) ? GetOrderStatus(APIResponse.txStatus) : APIResponse.orderStatus;
//                        res.Result.APIResponse = APIResponse;
//                        res.Result.ReferenceId = APIResponse.referenceId ?? string.Empty;
//                        res.Result.IsUpdateDb = true;
//                    }
//                    else
//                    {
//                        if (APIResponse.reason?.ToLower() == "order id does not exist")
//                        {
//                            res.StatusCode = ResponseStatus.Success;
//                            res.Result.OrderStatus = "Failed";
//                            res.Result.OrderId = orderId;
//                            res.Result.IsUpdateDb = true;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                string errorMsg = string.Concat(ex.Message, " | request : ", JsonConvert.SerializeObject(param), " | response : ", JsonConvert.SerializeObject(res.Result));
//                LogError(ex, errorMsg);

//            }
//            if (pgConfig.IsLoggingTrue)
//            {
//                APILog(pgConfig.StatusCheckURL, param.ToString(), cashfreeresp);
//                //_apiLogin.SaveLog(string.Concat(pgConfig.StatusCheckURL, "|", param), JsonConvert.SerializeObject(cashfreeresp), "Cashfree-->Statuscheck", orderId);
//            }
//            Finish:
//            return res;
//        }

//        private string GetOrderStatus(string txnStatus)
//        {
//            string sts = "Pending";
//            switch (txnStatus.ToUpper())
//            {
//                case "SUCCESS":
//                    sts = "SUCCESS";
//                    break;
//                case "FAILED":
//                    sts = "FAILED";
//                    break;
//                case "FAIL":
//                    sts = "FAILED";
//                    break;
//                case "USER_DROPPED":
//                    sts = "FAILED";
//                    break;
//            }
//            return sts;
//        }
//    }
//}