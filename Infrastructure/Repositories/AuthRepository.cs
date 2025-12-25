using Domain.Entities;
using Domain.Interfaces.RepositoryBase;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuthRepository : RepositoryBase<ApplicationUser>, IRepositoryBase<ApplicationUser>
    {
        public AuthRepository(EtolieEGDbContext context) : base(context)
        {
        }
    }
}
