using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
            
        Task SendEmailAsync(string to,string subject,string body);
    
    }
}
