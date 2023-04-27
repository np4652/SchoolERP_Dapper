using Data;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.AppCode.Helper;

namespace WebApp.Controllers
{
    [Route("/Class/")]
    public class ClassMasterController : Controller
    {
        private readonly IGenericMethods _httpRequest;
        public ClassMasterController(IGenericMethods httpRequest)
        {
            _httpRequest = httpRequest;
        }

        [HttpGet(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost(nameof(Save))]
        public async Task<IActionResult> Save(ClassMaster classMaster)
        {
            var res = await _httpRequest.Get<Response>($"Class/save", User?.GetLoggedInUserToken(), classMaster);
            return View(res);
        }

        [HttpGet(nameof(Edit))]
        public async Task<IActionResult> Edit(int id)
        {
            var res = await _httpRequest.Get<ClassMasterColumn>($"Class/GetById/{id}", User?.GetLoggedInUserToken(), new { });
            return PartialView(res);
        }

        [HttpPost(nameof(GetAll))]
        public async Task<IActionResult> GetAll(PagedRequest request)
        {
            var res = await _httpRequest.Get<PagedResult<ClassMasterColumn>>($"Class/GetAll", User?.GetLoggedInUserToken(), request);
            return PartialView(res ?? new PagedResult<ClassMasterColumn> { Data = new List<ClassMasterColumn>() });
        }
    }
}
