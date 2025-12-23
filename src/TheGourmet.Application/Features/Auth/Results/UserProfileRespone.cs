using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGourmet.Application.Features.Auth.Results
{
    public class UserProfileRespone
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}