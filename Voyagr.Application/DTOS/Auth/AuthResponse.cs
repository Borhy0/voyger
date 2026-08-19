using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;

        public UserResponse User { get; set; } = new();
    }

    public class UserResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PreferredCurrency { get; set; } = string.Empty;

        public string Units { get; set; } = string.Empty;
    }
}
