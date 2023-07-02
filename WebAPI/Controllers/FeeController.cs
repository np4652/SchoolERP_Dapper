using Data;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/student/")]
    public class FeeController : ControllerBase
    {
        private readonly IFeeService _feeService;

        public FeeController(IFeeService feeService)
        {
            _feeService=feeService;
        }

        [HttpPost(nameof(FeeHistory))]
        public async Task<IActionResult> FeeHistory(PagedRequest filter)
        {
            var res = await _feeService.FeeHistory(filter);
            return Ok(res);
        }

        [HttpPost(nameof(EstimatedFee))]
        public async Task<IActionResult> EstimatedFee(int studentId, int classId)
        {
            var res = await _feeService.EstimatedFee(studentId, classId);
            return Ok(res);
        }
    }
}