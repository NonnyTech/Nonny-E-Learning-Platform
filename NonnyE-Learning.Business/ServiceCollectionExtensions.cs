using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Helper;

namespace NonnyE_Learning.Business
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<ICourseServices, CourseService>();
            services.AddScoped<IEmailServices, EmailServices>();
            services.AddScoped<ITransactionServices, TransactionServices>();
            services.AddScoped<IEnrollmentServices, EnrollmentServices>();
            services.AddScoped<IModuleServices, ModuleServices>();
            services.AddScoped<ICertificateService, CertificateServices>();
            services.AddScoped<IPricingPlanServices, PricingPlanServices>();
            services.AddHttpClient<IFlutterwaveServices, FlutterwaveServices>();
            services.AddScoped<IUserTokenService, UserTokenService>();

            return services;
        }
    }
}