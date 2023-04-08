using Data;
using Entities.Models;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/Session/")]
    public class SessionMasterController : ControllerBase
    {
        private readonly ISessionMasterService _sessionMasterService;
        public SessionMasterController(ISessionMasterService sessionMasterService)
        {
            _sessionMasterService=sessionMasterService;
        }

        [HttpPost("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _sessionMasterService.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPost(nameof(GetAll))]
        public async Task<IActionResult> GetAll(PagedRequest request)
        {
            PagedResult<SessionMasterColumn> data = await _sessionMasterService.GetAsync(request);
            return Ok(data);
        }

        [HttpPost(nameof(Save))]
        public async Task<IActionResult> Save(SessionMaster sessionMaster)
        {
            var res = await _sessionMasterService.AddAsync(sessionMaster);
            return Ok(res);
        }
    }
}
