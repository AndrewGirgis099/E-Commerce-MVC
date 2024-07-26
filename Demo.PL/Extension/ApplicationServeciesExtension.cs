using Demo.BLL.Interfacies;
using Demo.BLL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.PL.Extension
{
    public static class ApplicationServeciesExtension
    {
        public static IServiceCollection AddApplicationServicies(this IServiceCollection services)
        {
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            return services;
        }
    }
}
