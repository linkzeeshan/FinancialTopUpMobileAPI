using MobileTopUpAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models.Dtos
{
    public class BeneficiaryCreateDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Nickname is required")]
        [StringLength(20, ErrorMessage = "Beneficiary nickname cannot be longer than 20 characters")]
        public string Nickname { get; set; }

        public bool? IsActive { get; set; }

    }
    public class BeneficiaryReadDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Nickname { get; set; }
        public bool IsActive { get; set; }
        // Other beneficiary properties

        // Navigation property
        //public User User { get; set; }

    }
}
