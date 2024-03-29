﻿using Data;
using Entities.Models;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/student/")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("/GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _studentService.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPost(nameof(GetAll))]
        public async Task<IActionResult> GetAll(PagedRequest request)
        {
            PagedResult<StudentColumn> data = await _studentService.GetAsync(request);
            return Ok(data);
        }


        [HttpPost(nameof(Save))]
        public async Task<IActionResult> Save(Student student)
        {
            var res = await _studentService.AddAsync(student);
            return Ok(res);
        }

        

        [HttpPost(nameof(GetSessionWiseStudent))]
        public async Task<IActionResult> GetSessionWiseStudent(PagedRequest request)
        {
            PagedResult<StudentColumn> data = await _studentService.SessionWiseStudent(request);
            return Ok(data);
        }
    }
}
