using Data;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.AppCode.Helper;

namespace WebApp.Controllers
{
    [Route("/Fee/")]
    public class FeeController : Controller
    {
        private readonly IGenericMethods _httpRequest;
        public FeeController(IGenericMethods httpRequest)
        {
            _httpRequest = httpRequest;
        }

        [HttpGet(nameof(FeeHistory))]
        public async Task<IActionResult> FeeHistory()
        {   
            return View();
        }

        [HttpPost(nameof(_FeeHistory))]
        public async Task<IActionResult> _FeeHistory(PagedRequest request)
        {
            var res = await _httpRequest.Get<PagedResult<FeeHistory>>($"Fee/FeeHistory", User?.GetLoggedInUserToken(), request);
            return PartialView(res ?? new PagedResult<FeeHistory> { Data = new List<FeeHistory>() });
        }

        [HttpPost(nameof(EstimatedFee))]
        public async Task<IActionResult> EstimatedFee(int studentId, int classId)
        {
            var res = await _httpRequest.Get<List<FeeVaribale>>($"Fee/EstimatedFee?studentId={studentId}&classId={classId}", User?.GetLoggedInUserToken());
            return PartialView(res ?? new List<FeeVaribale>());
        }
    }
}
