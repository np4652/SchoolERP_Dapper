using Data;
using Entities.Models;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/Feemaster/")]
    public class FeeMasterController : ControllerBase
    {
        private readonly IFeeGroupService _feeGroupService;
        private readonly IFeeVaribaleService _feeVaribaleService;
        public FeeMasterController(IFeeGroupService feeGroupService, IFeeVaribaleService feeVaribaleService)
        {
            _feeGroupService = feeGroupService;
            _feeVaribaleService = feeVaribaleService;
        }
        [HttpPost("/Feegroup/GetById/{id}")]
        public async Task<IActionResult> GetFeegroupById(int id)
        {
            var data = await _feeGroupService.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPost("Feegroup/GetAll")]
        public async Task<IActionResult> GetAllFeegroup(PagedRequest request)
        {
            PagedResult<FeeGroupColumn> data = await _feeGroupService.GetAsync(request);
            return Ok(data);
        }


        [HttpPost("Feegroup/Save")]
        public async Task<IActionResult> SaveFeegroup(FeeGroup feeGroup)
        {
            var res = await _feeGroupService.AddAsync(feeGroup);
            return Ok(res);
        }

        [HttpPost("/Feevariable/GetById/{id}")]
        public async Task<IActionResult> GetFeevariableById(int id)
        {
            var data = await _feeVaribaleService.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpPost("Feevariable/GetAll")]
        public async Task<IActionResult> GetAllFeevariable(PagedRequest request)
        {
            PagedResult<FeeVaribaleColumn> data = await _feeVaribaleService.GetAsync(request);
            return Ok(data);
        }


        [HttpPost("Feevariable/Save")]
        public async Task<IActionResult> SaveFeevariable(FeeVaribale feeVaribale)
        {
            var res = await _feeVaribaleService.AddAsync(feeVaribale);
            return Ok(res);
        }
    }
}
