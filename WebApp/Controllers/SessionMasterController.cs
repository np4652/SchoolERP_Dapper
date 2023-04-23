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
    [Route("/Session/")]
    public class SessionMasterController : Controller
    {
        private readonly IGenericMethods _httpRequest;
        public SessionMasterController(IGenericMethods httpRequest)
        {
            _httpRequest = httpRequest;
        }

        [HttpGet(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost(nameof(Save))]
        public async Task<IActionResult> Save(SessionMaster sessionMaster)
        {
            var res = await _httpRequest.Get<Response>($"Session/save", User?.GetLoggedInUserToken(), sessionMaster);
            return View(res);
        }

        [HttpPost(nameof(Edit))]
        public async Task<IActionResult> Edit(int id)
        {
            var res = await _httpRequest.Get<SessionMasterColumn>($"Session/GetById/{id}", User?.GetLoggedInUserToken(), new { });
            return PartialView(res);
        }

        [HttpPost(nameof(GetAll))]
        public async Task<IActionResult> GetAll(PagedRequest request)
        {
            var res = await _httpRequest.Get<PagedResult<ClassMasterColumn>>($"Session/GetAll", User?.GetLoggedInUserToken(), request);
            return PartialView(res ?? new PagedResult<ClassMasterColumn> { Data = new List<ClassMasterColumn>() });
        }
    }
}
