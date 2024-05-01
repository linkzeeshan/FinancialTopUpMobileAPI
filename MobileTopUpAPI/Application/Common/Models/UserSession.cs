using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public record UserSession(string? Id, string? Name, string? Email, string? Role);
}
