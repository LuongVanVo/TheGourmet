using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGourmet.Application.Common.Events
{
    public class UserRegisteredEvent
    {
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}