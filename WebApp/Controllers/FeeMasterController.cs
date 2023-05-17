using Data;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.AppCode.Helper;

namespace WebApp.Controllers
{
    [Route("/Feemaster/")]
    public class FeeMasterController : Controller
    {
        private readonly IGenericMethods _httpRequest;
        public FeeMasterController(IGenericMethods httpRequest)
        {
            _httpRequest = httpRequest;
        }

        [HttpGet(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost(nameof(Save))]
        public async Task<IActionResult> Save(FeeGroup feeGroup)
        {
            var res = await _httpRequest.Get<Response>($"Feemaster/Feegroup/Save", User?.GetLoggedInUserToken(), feeGroup);
            return Json(res);
        }

        [HttpPost(nameof(Edit))]
        public async Task<IActionResult> Edit(int id)
        {

            var res = await _httpRequest.Get<FeeGroupColumn>($"Feemaster/Feegroup/GetById/{id}", User?.GetLoggedInUserToken(), new { });
            return PartialView(res);
        }

        [HttpPost(nameof(GetAll))]
        public async Task<IActionResult> GetAll(PagedRequest request)
        {
            var res = await _httpRequest.Get<PagedResult<FeeGroupColumn>>($"Feemaster/Feegroup/GetAll", User?.GetLoggedInUserToken(), request);
            return PartialView(res ?? new PagedResult<FeeGroupColumn> { Data = new List<FeeGroupColumn>() });
        }
        //---------------------------For Feevariable-----------------------//

        [HttpPost($"FeeVaribale/{nameof(Save)}")]
        public async Task<IActionResult> Save(FeeVaribale FeeGroup)
        {
            var res = await _httpRequest.Get<Response>($"Feemaster/Feevariable/Save", User?.GetLoggedInUserToken(), FeeGroup);
            return View(res);
        }

        [HttpPost("FeeVaribale/" + nameof(Edit) + "/{id}")]
        public async Task<IActionResult> EditFeeVaribale(int id)
        {

            var res = await _httpRequest.Get<FeeVaribaleColumn>($"Feemaster/Feevariable/GetById/{id}", User?.GetLoggedInUserToken(), new { });
            return PartialView(res);
        }

        [HttpPost($"FeeVaribale/{nameof(GetAll)}")]
        public async Task<IActionResult> GetAllFeeVaribale(PagedRequest request)
        {
            var res = await _httpRequest.Get<PagedResult<FeeVaribaleColumn>>($"Feemaster/Feevariable/GetAll", User?.GetLoggedInUserToken(), request);
            return PartialView("GetAll",res ?? new PagedResult<FeeVaribaleColumn> { Data = new List<FeeVaribaleColumn>() });
        }
    }
}
