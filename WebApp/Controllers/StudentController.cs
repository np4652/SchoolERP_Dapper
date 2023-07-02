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
    [Route("/student/")]
    public class StudentController : Controller
    {
        private readonly IGenericMethods _httpRequest;
        public StudentController(IGenericMethods httpRequest)
        {
            _httpRequest = httpRequest;
        }

        [HttpGet(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost(nameof(Save))]
        public async Task<IActionResult> Save(Student student)
        {
            var res = await _httpRequest.Get<Response>($"student/save", User?.GetLoggedInUserToken(), student);
            return View(res);
        }

        [HttpPost(nameof(Edit))]
        public async Task<IActionResult> Edit(int id)
        {
            var res = await _httpRequest.Get<Student>($"student/GetById/{id}", User?.GetLoggedInUserToken(), new { });
            return PartialView(res ?? new Student());
        }

        [HttpPost(nameof(GetAll))]
        public async Task<IActionResult> GetAll(PagedRequest request)
        {
            var res = await _httpRequest.Get<PagedResult<StudentColumn>>($"student/GetAll", User?.GetLoggedInUserToken(), request);
            return PartialView(res ?? new PagedResult<StudentColumn> { Data = new List<StudentColumn>() });
        }

        [HttpGet(nameof(SessionWiseStudent))]
        public async Task<IActionResult> SessionWiseStudent(PagedRequest request)
        {
            return View();
        }

        [HttpPost(nameof(_SessionWiseStudent))]
        public async Task<IActionResult> _SessionWiseStudent(PagedRequest request)
        {
            var res = await _httpRequest.Get<PagedResult<StudentColumn>>($"student/GetSessionWiseStudent", User?.GetLoggedInUserToken(), request);
            return PartialView(res ?? new PagedResult<StudentColumn> { Data = new List<StudentColumn>() });
        }
    }
}
