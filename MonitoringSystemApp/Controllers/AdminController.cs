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

        private readonly IMemberService _memberService;
        private readonly ILabService _labService;

        public AdminController(
            IRegistrationRequestService RegistrationRequestService,
            IDashboardAccountService DashboardAccountService,
            IMemberService memberService,
            ILabService labService
            ) {
            _RegistrationRequestService = RegistrationRequestService;
            _DashboardAccountService = DashboardAccountService;
            _memberService = memberService;
            _labService = labService;
        }

        [HttpGet("RegistrationRequests")]

        public async Task<ActionResult<PagedResponse<RegistrationResponseDto>>> GetAllRegistrationRequests(
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

        [HttpGet("Members")]

        public async Task<ActionResult<PagedResponse<MemberResponse>>> GetAllMembers(
            [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            var result = await _memberService.GetAllAsync(pageNumber,pageSize);
            return Ok(result);
        }


        [HttpGet("Labs")]

        public async Task<ActionResult<PagedResponse<LabResponse>>> GetAllLabs(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _labService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }



        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount(DashboardAccountRequest acc)
        {
            await _DashboardAccountService.CreateAsync(acc);
            return NoContent();
        }

        [HttpPost("CreateLab")]
        public async Task<IActionResult> CreateLab(LabRequest lab)
        {
            await _labService.CreateAsync(lab);
            return NoContent();
        }


        [HttpDelete("delete-member/{id}")]
        public async Task<IActionResult> DeleteMember(int id) { 
            await _memberService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("delete-acc/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            await _DashboardAccountService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("delete-lab/{id}")]
        public async Task<IActionResult> DeleteLab(int id)
        {
            await _labService.DeleteAsync(id);
            return NoContent();
        }



        [HttpPatch("RequestReject/{id}")]
        public async Task<IActionResult> RejectRegistrationRequest(int id)
        {
            await _RegistrationRequestService.MarkStatus(id, false);
            return NoContent();
        }

        [HttpPatch("RequestApprove/{id}")]
        public async Task<IActionResult> ApproveRegistrationRequest(int id)
        {
            await _RegistrationRequestService.MarkStatus(id, true);
            return NoContent();
        }

        [HttpPatch("DeactivatAccount/{id}")]
        public async Task<IActionResult> DeactivateAccount(int id)
        {
            await _DashboardAccountService.Deactivate(id);
            return NoContent();
        }

        [HttpPatch("ActivatAccount/{id}")]
        public async Task<IActionResult> ActivateAccount(int id)
        {
            await _DashboardAccountService.Activate(id);
            return NoContent();
        }
    }
}
