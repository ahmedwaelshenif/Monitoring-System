using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ResponseDtos
{
    public class MemberResponseDto
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
        
        [Range(1, 4)]
        public int AcademicYear { get; set; }

        public IFormFile FaceImage { get; set; }
   
        public IFormFile IdImage { get; set; }
        
    }
}
