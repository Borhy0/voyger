using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyagr.Domain.Entities;

namespace Voyagr.Application.Interfaces
{
    public interface ITripRepository
    {
        Task<Trip?> GetByIdAsync(Guid id);

        Task<List<Trip>> GetByUserIdAsync(Guid userId);

        Task AddAsync(Trip trip);

        void Update(Trip trip);

        Task SaveChangesAsync();
    }
}
