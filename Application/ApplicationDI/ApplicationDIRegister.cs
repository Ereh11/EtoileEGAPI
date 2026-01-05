using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ApplicationDI
{
    public static class ApplicationDIRegister
    {
        public static void AddApplicationDIRegister(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
                typeof(ApplicationDIRegister).Assembly)
            );
            services.AddValidatorsFromAssembly(
                typeof(ApplicationDIRegister).Assembly
            );
        }
    }
}
