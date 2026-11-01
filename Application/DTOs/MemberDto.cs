using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
        
        [Range(1, 4)]
        public int AcademicYear { get; set; }

        public byte[] FaceImage { get; set; }
   
        public byte[] IdImage { get; set; }
        
        public byte[] EmbeddingData { get; set; }
    }
}
