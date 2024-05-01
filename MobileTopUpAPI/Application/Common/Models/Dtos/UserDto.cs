using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models.Dtos
{
    public class UserCreateDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsVerified { get; set; } = true;


    }

    public class UserReadDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsVerified { get; set; } = true;


    }
}
