using Data;
using Entities.Models;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/Class/")]
    public class ClassMasterController : ControllerBase
    {
        private readonly IClassMasterService _classMasterService;
        public ClassMasterController(IClassMasterService classMasterService)
        {
            _classMasterService = classMasterService;
        }

        [HttpPost("GetById/{id}")]
        public async Task<IActionResult> GetById(int id = 0)
        {
            ClassMasterColumn data = new ClassMasterColumn();
            if (id>0)
            {
                data = await _classMasterService.GetByIdAsync(id);
            }
            return Ok(data);
        }

        [HttpPost(nameof(GetAll))]
        public async Task<IActionResult> GetAll(PagedRequest request)
        {
            PagedResult<ClassMasterColumn>  data = await _classMasterService.GetAsync(request);
            return Ok(data);
        }

        [HttpPost(nameof(Save))]
        public async Task<IActionResult> Save(ClassMaster classMaster)
        {
            var res = await _classMasterService.AddAsync(classMaster);
            return Ok(res);
        }
    }
}