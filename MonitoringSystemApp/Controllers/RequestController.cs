using Application.DTOs;
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
    [Route("requests")]
    public class RequestsController : ControllerBase
    {


        private readonly IRegistrationRequestService _RegistrationRequestService;

        public RequestsController(IRegistrationRequestService RegistrationRequestService)
        {
            _RegistrationRequestService = RegistrationRequestService;
        }

        [HttpPost("Send")]
        public async Task<ActionResult> Add([FromBody] RegistrationRequestDto request)
        {
            await _RegistrationRequestService.CreateAsync(request);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
