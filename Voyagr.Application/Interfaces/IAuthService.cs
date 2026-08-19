using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyagr.Application.DTOS.Auth;

namespace Voyagr.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> SignupAsync(
        SignupRequest request
    );

    Task<AuthResponse> LoginAsync(
        LoginRequest request
    );

    Task<UserResponse> GetCurrentUserAsync(
        Guid userId
    );
}