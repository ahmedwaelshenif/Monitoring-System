using Application.DTOs;
using Application.DTOs.ResponseDtos;
using Application.DTOs.RequestDtos;
using Application.Interfaces.Services;
using Application.Services;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonitoringSystemApp.Controllers
{

    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {


        private readonly IRegistrationRequestService _RegistrationRequestService;
        private readonly IDashboardAccountService _DashboardAccountService;

        public AdminController(IRegistrationRequestService RegistrationRequestService, IDashboardAccountService DashboardAccountService) {
            _RegistrationRequestService = RegistrationRequestService;
            _DashboardAccountService = DashboardAccountService;
        }

        [HttpGet("RegistrationRequests")]

        public async Task<ActionResult<PagedResponse<RegistrationRequestDto>>> GetAllRegistrationRequests(
            [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            var result = await _RegistrationRequestService.GetAllAsync(pageNumber,pageSize);
            return Ok(result);
        }

        [HttpGet("DashboardAccount")]

        public async Task<ActionResult<PagedResponse<DashboardAccountResponse>>> GetAllDashboardAccounts(
            [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            var result = await _DashboardAccountService.GetAllAsync(pageNumber,pageSize);
            return Ok(result);
        }


        [HttpPatch("RequestReject/{id}")]
        public async Task<IActionResult> MarkRegistratioRequestAsRejected(int id)
        {
            await _RegistrationRequestService.MarkStatus(id, false);
            return NoContent();
        }

        [HttpPatch("RequestApprove/{id}")]
        public async Task<IActionResult> MarkRegistratioRequestAsApproved(int id)
        {
            await _RegistrationRequestService.MarkStatus(id, true);
            return NoContent();
        }

        [HttpPatch("DeactivatAccount/{id}")]
        public async Task<IActionResult> DeactivatAccount(int id)
        {
            await _DashboardAccountService.Deactivate(id);
            return NoContent();
        }

        [HttpPatch("ActivatAccount/{id}")]
        public async Task<IActionResult> ActivatAccount(int id)
        {
            await _DashboardAccountService.Activate(id);
            return NoContent();
        }
    }
}
