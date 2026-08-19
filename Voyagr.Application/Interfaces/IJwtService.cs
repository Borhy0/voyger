using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyagr.Domain.Entities;


namespace Voyagr.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
