using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGourmet.Application.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}