using AppUtility.Extensions;
using AppUtility.Helper;
using Data;
using Entities.Enums;
using Entities.Models;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.Identity;
using Service.PaymentGateway;
using System.Text;
using System.Text.RegularExpressions;
using WebAPI.Helpers;
using WebAPI.Models;
using static Service.PaymentGateway.CashFree.Models;

namespace WebAPI.Controllers
{

    public class CallbackController : Controller
    {
        private readonly IPaymentGatewayService _pgService;
        private readonly IPGDetailService _pgDetail;
        private readonly IUserService _userService;
        private readonly IEmailConfiguration _emailConfiguration;
        public CallbackController(IPaymentGatewayService pgService, IUserService userService, IPGDetailService pgDetail, IEmailConfiguration emailConfiguration)
        {
            _pgService = pgService;
            _userService = userService;
            _pgDetail = pgDetail;
            _emailConfiguration=emailConfiguration;
        }

        #region CashFree
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("/CashFreeStatusCheck")]
        public async Task<IActionResult> CashFreeStatusCheck(string tid)
        {
            tid = tid?.ToUpper().Replace("TID", "").Replace("K", "") ?? "0";
            var res = await _pgService.StatusCheck(new StatusCheckRequest
            {
                TID = Convert.ToInt32(tid),
                PGID = PaymentGatewayType.CASHFREE,
            });
            res.Result.OrderStatus = res.Result.OrderStatus[0].ToString();
            res.Result.APIResponse = null;
            Regex r = new Regex(@"^(TID)(\d+)(K$)");
            if (!r.IsMatch(res.Result.OrderId))
            {
                res.Result.OrderId =  $"TID{tid.ToString().PadLeft(5, '0')}K";
            }
            if (res.StatusCode == ResponseStatus.Success && res.Result.IsUpdateDb)
            {
                //var _res = await _pgDetail.UpdateTransactionStatus(res.Result.OrderStatus, Convert.ToInt32(tid), res.Result.ReferenceId);
            }
            _pgService.APILog("/CashFreenotify", tid, JsonConvert.SerializeObject(res));
            return Json(res);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/CashFreereturn")]
        public async Task<IActionResult> CashFreereturn(string order_id, string order_token)
        {
            int tid = Convert.ToInt32(order_id?.ToUpper().Replace("TID", "").Replace("K", ""));
            var res = await _pgService.StatusCheck(new StatusCheckRequest
            {
                TID = tid,
                PGID = PaymentGatewayType.CASHFREE,
            });
            /* Assign package and update DB*/
            if (res.StatusCode == ResponseStatus.Success && res.Result.OrderStatus.ToUpper().In("S", "SUCCESS"))
            {
                //var response = await _userService.Assignpackage(tid);
            }
            /* End */
            StringBuilder html = new StringBuilder(@"<html><head><script>
                                (()=>{
                                        var obj={TID:""{TID}"",Amount:""{Amount}"",TransactionID:""{TransactionID}"",statusCode:""{statusCode}"",reason:""{reason}"",origin:""addMoney"",gateway:""CashFree""}
                                        localStorage.setItem('obj', JSON.stringify(obj));
                                        window.close()
                                   })();</script></head><body><h6>Redirect to site.....</h6></body></html>");
            html.Replace("{TID}", res.Result.OrderId);
            html.Replace("{Amount}", "");
            html.Replace("{TransactionID}", res.Result.OrderId);
            html.Replace("{statusCode}", ((int)res.StatusCode).ToString()); // comesfrom DB
            html.Replace("{reason}", res.Result.OrderStatus);
            return Content(html.ToString(), contentType: "text/html; charset=utf-8");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, HttpPost]
        [Route("/CashFreenotify")]
        public async Task<IActionResult> CashFreenotify()
        {
            var httpRequest = await AppHelper.O.ExtractHttpContextRequestAsync(HttpContext.Request);
            var callbackAPIReq = new CallbackData
            {
                Method = httpRequest.Method,
                APIID = 0,
                Content = httpRequest.Content,
                Scheme = httpRequest.Scheme,
                Path = httpRequest.Path
            };
            _pgService.APILog("/CashFreenotify", callbackAPIReq.Content, "");
            try
            {
                var cfCData = JsonConvert.DeserializeObject<CFRealCallbackResp>(callbackAPIReq.Content) ?? new CFRealCallbackResp
                {
                    data=new CFData
                    {
                        order = new CFOrder(),
                        payment = new CFPayment(),
                    }
                };
                var _res = await UpdateCashfreeResponse(new CashfreeCallbackResponse
                {
                    orderId = cfCData.data.order.order_id,
                    orderAmount = Convert.ToDecimal(cfCData.data.order.order_amount),
                    referenceId = cfCData.data.payment.bank_reference,
                    txStatus = cfCData.data.payment.payment_status,
                    paymentMode = cfCData.data.payment.payment_group,
                    txMsg = cfCData.data.payment.payment_message,
                    txTime = cfCData.data.payment.payment_time,
                    signature = "",
                });
                return Ok(_res);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message, new { className = this.GetType().Name, fn = nameof(CashFreenotify) });
                return BadRequest("Something went wrong");
            }

        }

        /* have to Edit 
         * 1. check status by API
         * 2. Replace TID from orderId and get only numeric value
         * 3. ProcGetTransactionPGDetail by passing commonInt instead of commonStr
         * 4. check amount get by  status check with amount get by proc
         * 5. create a private function to modify  PaymentMode according db
     */
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response> UpdateCashfreeResponse(CashfreeCallbackResponse PGResponse)
        {
            var res = new Response
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = ResponseStatus.Failed.ToString()
            };
            try
            {
                if (PGResponse == null)
                    return res;
                if (string.IsNullOrEmpty(PGResponse.orderId))
                    return res;
                if (string.IsNullOrEmpty(PGResponse.referenceId))
                    return res;
                if (PGResponse.orderAmount <= 0)
                    return res;
                if (string.IsNullOrEmpty(PGResponse.paymentMode))
                    return res;
                if (string.IsNullOrEmpty(PGResponse.txStatus))
                    return res;
                
                int TID = !string.IsNullOrEmpty(PGResponse.orderId) ? Convert.ToInt32(PGResponse.orderId?.ToUpper().Replace("TID","").ToUpper().Replace("K","")) : 0;

                var procRes = await _pgService.GetInitiatedPaymentDetail(TID);
                if (procRes.StatusCode == ResponseStatus.Success)
                {
                    if (procRes.Result.Status?.ToUpper()=="P")//procRes.Status.In(RechargeRespType.REQUESTSENT, RechargeRespType.PENDING)
                    {
                        //var appId = procRes.Result.MerchantID;
                        //var secretKey = procRes.Result.MerchantKey;
                        var orderId = procRes.Result.TID;
                        if (procRes!=null && procRes.Result!=null)
                            if (procRes.Result.PGID == PaymentGatewayType.CASHFREE)
                            {
                                var stsCheckResp = await _pgService.StatusCheck(new StatusCheckRequest
                                {
                                    TID = Convert.ToInt32(procRes.Result.TID),
                                    PGID = procRes.Result.PGID
                                });
                                res.StatusCode = stsCheckResp.StatusCode;
                                res.ResponseText = stsCheckResp.ResponseText;
                                if (stsCheckResp != null && stsCheckResp.StatusCode!=ResponseStatus.Error)
                                {
                                    if (procRes.Result.Amount == stsCheckResp.Result.OrderAmount)
                                    {
                                        var PayMethod = PGResponse.paymentMode;
                                        string status = "P";
                                        stsCheckResp.Result.OrderStatus = stsCheckResp.Result.OrderStatus ?? string.Empty;
                                        PGResponse.txStatus = PGResponse.txStatus ?? string.Empty;
                                        if (stsCheckResp.Result.OrderStatus.ToUpper().In("OK", "S", "PAID", "SUCCESS") && PGResponse.txStatus.ToUpper() == "SUCCESS")
                                        {
                                            status = "S";
                                        }
                                        else if (PGResponse.txStatus.ToUpper()=="FAILED")
                                        {
                                            status = "F";
                                        }
                                        else
                                        {
                                            var apiResponse = (CashfreeStatusResponse)stsCheckResp.Result.APIResponse;
                                            if ((stsCheckResp.Result.OrderStatus.Equals("error", StringComparison.OrdinalIgnoreCase) && apiResponse.reason.Contains("Order Id does not exist", StringComparison.OrdinalIgnoreCase)))
                                            {
                                                status = "F";
                                            }
                                        }
                                        /* update status in DB */
                                        res = await _pgService.updateInitiatedPayment(TID, status);
                                    }
                                }
                            }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }
        #endregion

        #region AllUPI
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("/AllUPIStatusCheck")]
        public async Task<IActionResult> AllUPIStatusCheck(int TID)
        {
            var res = await _pgService.StatusCheck(new StatusCheckRequest
            {
                TID = TID,
                PGID = PaymentGatewayType.MitraUPINEW,
            });
            if (res.StatusCode == ResponseStatus.Success && res.Result.IsUpdateDb)
            {
                //var _res = await _pgDetail.UpdateTransactionStatus(res.Result.OrderStatus, TID, res.Result.ReferenceId);
            }
            return Json(res);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/AllUPIreturn")]
        public async Task<IActionResult> AllUPIreturn(string requestedId, string tid)
        {
            tid = (requestedId?.Replace("TID", ""));
            var res = await _pgService.StatusCheck(new StatusCheckRequest
            {
                TID = Convert.ToInt32(tid),
                PGID = PaymentGatewayType.MitraUPINEW,
            });
            /* Assign package and update DB*/
            if (res != null && res.StatusCode == ResponseStatus.Success && res.Result.OrderStatus.ToUpper().In("S", "SUCCESS"))
            {
                //var response = await _userService.Assignpackage(Convert.ToInt32(tid));
            }
            /* End */
            StringBuilder html = new StringBuilder(@"<html><head><script>
                                (()=>{
                                        var obj={TID:""{TID}"",Amount:""{Amount}"",TransactionID:""{TransactionID}"",statusCode:""{statusCode}"",reason:""{reason}"",origin:""addMoney"",gateway:""CashFree""}
                                        localStorage.setItem('obj', JSON.stringify(obj));
                                        window.close()
                                   })();</script></head><body><h6>Redirect to site.....</h6></body></html>");
            html.Replace("{TID}", res.Result.OrderId);
            html.Replace("{Amount}", "");
            html.Replace("{TransactionID}", res.Result.OrderId);
            html.Replace("{statusCode}", ((int)res.StatusCode).ToString()); // comesfrom DB
            html.Replace("{reason}", res.Result.OrderStatus);
            return Content(html.ToString(), contentType: "text/html; charset=utf-8");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response> UpdateAllUPIResponse(CashfreeCallbackResponse PGResponse)
        {
            var res = new Response
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = ResponseStatus.Failed.ToString()
            };
            try
            {
                if (PGResponse == null)
                    return res;
                if (string.IsNullOrEmpty(PGResponse.orderId))
                    return res;
                if (string.IsNullOrEmpty(PGResponse.referenceId))
                    return res;
                if (PGResponse.orderAmount <= 0)
                    return res;
                if (string.IsNullOrEmpty(PGResponse.paymentMode))
                    return res;
                if (string.IsNullOrEmpty(PGResponse.txStatus))
                    return res;

                int TID = !string.IsNullOrEmpty(PGResponse.orderId) ? Convert.ToInt32(PGResponse.orderId.Replace("TID", string.Empty, StringComparison.OrdinalIgnoreCase)) : 0;

                var procRes = await _pgService.GetInitiatedPaymentDetail(TID);
                if (procRes.StatusCode == ResponseStatus.Success)
                {
                    if (true)//procRes.Status.In(RechargeRespType.REQUESTSENT, RechargeRespType.PENDING)
                    {
                        var appId = procRes.Result.MerchantID;
                        var secretKey = procRes.Result.MerchantKey;
                        var orderId = procRes.Result.TID;
                        if (procRes.Result.PGID == PaymentGatewayType.MitraUPINEW)
                        {
                            var stsCheckResp = await _pgService.StatusCheck(new StatusCheckRequest
                            {
                                TID = Convert.ToInt32(procRes.Result.TID),
                                PGID = procRes.Result.PGID
                            });
                            if (stsCheckResp != null)
                            {
                                if (procRes.Result.Amount == stsCheckResp.Result.OrderAmount)
                                {
                                    var PayMethod = PGResponse.paymentMode;
                                    string status = "P";
                                    if (stsCheckResp.Result.OrderStatus.Equals("OK", StringComparison.OrdinalIgnoreCase) && PGResponse.txStatus.Equals("success", StringComparison.OrdinalIgnoreCase))
                                    {
                                        status = "S";
                                    }
                                    else if (PGResponse.txStatus.Equals("failed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        status = "F";
                                    }
                                    else
                                    {
                                        var apiResponse = (CashfreeStatusResponse)stsCheckResp.Result.APIResponse;
                                        if ((stsCheckResp.Result.OrderStatus.Equals("error", StringComparison.OrdinalIgnoreCase) && apiResponse.reason.Contains("Order Id does not exist", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            status = "F";
                                        }
                                    }
                                    /* update status in DB */
                                    res = await _pgService.updateInitiatedPayment(TID, status);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }
        #endregion

        [HttpPost(nameof(SendMail))]
        public async Task<IActionResult> SendMail([FromBody] MailRequest request)
        {
            var res = await _emailConfiguration.GetAllAsync();
            if (res != null)
            {
                var first = res.FirstOrDefault();
                if (first != null)
                {
                    EmailSettings settings = new EmailSettings
                    {
                        EmailFrom= first.EmailFrom,
                        EmailTo = request.To,
                        IsSSL = first.IsSSL,
                        HostName= first.HostName,
                        Id=first.Id,
                        Password= first.Password,
                        Port= first.Port,
                        Subject = request.Subject,
                        UserId = first.UserId,
                        Body= request.Body,
                    };
                    Utility.O.SendMail(settings);

                }
            }
            return Ok(new Response
            {
                StatusCode = ResponseStatus.Success,
                ResponseText = "Mail Send successfully"
            });
        }
    }
}
