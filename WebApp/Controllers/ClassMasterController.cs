using Data;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.AppCode.Helper;

namespace WebApp.Controllers
{
    public class ClassMasterController : Controller
    {
        private readonly IGenericMethods _httpRequest;
        public ClassMasterController(IGenericMethods httpRequest)
        {
            _httpRequest = httpRequest;
        }

        public async Task<IActionResult> Save(ClassMaster classMaster)
        {
            var res = await _httpRequest.Get<Response>($"/api/Class/GetAll", User?.GetLoggedInUserToken(), classMaster);
            return View(res);
        }
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var res = await _httpRequest.Get<ClassMasterColumn>($"/api/Class/GetById/{id}", User?.GetLoggedInUserToken(), new { });
            return View(res);
        }

        public async Task<IActionResult> GetAll(PagedRequest request)
        {
            var res = await _httpRequest.Get<ClassMasterColumn>($"/api/Class/GetAll", User?.GetLoggedInUserToken(), request);
            return View(res);
        }
    }
}
