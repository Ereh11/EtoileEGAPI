using Domain.Configurations;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.InfraStructureDI
{
    public static class InfraStructureDIRegister
    {
        public static void AddInfraStructureDIRegister(this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddDbContext<EtolieEGDbContext>(options => options.UseSqlServer(Config.Read_DefaultConnection));
            services.AddTransient<UnitOfWork>();
        }
    }
}
