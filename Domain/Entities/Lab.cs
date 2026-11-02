using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Domain.Entities
{
    public class Lab
    {
        [Key]
        public int Id { get;set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Capacity { get; set; }


        public ICollection<Member> Members { get; set; } = new List<Member>();

    }
}
