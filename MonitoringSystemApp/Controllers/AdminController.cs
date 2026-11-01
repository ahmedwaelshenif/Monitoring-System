using Application.DTOs;
using Application.Interfaces;
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

        public AdminController(IRegistrationRequestService RegistrationRequestService) {
            _RegistrationRequestService = RegistrationRequestService;
        }

        [HttpGet("Requests")]

        public async Task<ActionResult<IEnumerable<RegistrationRequestDto>>> GetAllRequests()
        {
            var request= await _RegistrationRequestService.GetAllAsync();
            return Ok(request); 
        }


        [HttpPatch("Reject/{id}")]
        public async Task<IActionResult> MarkAsRejected(int id)
        {
            await _RegistrationRequestService.MarkStatus(id, false);
            return NoContent();
        }

        [HttpPatch("Approve/{id}")]
        public async Task<IActionResult> MarkAsApproved(int id)
        {
            await _RegistrationRequestService.MarkStatus(id, true);
            return NoContent();
        }
    }
}
