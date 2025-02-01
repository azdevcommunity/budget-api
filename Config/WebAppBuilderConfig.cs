using System.Reflection;
using AzDev.Core.APIDocumentation.Swagger.Extensions;
using AzDev.Core.Configurations.Extensions;
using AzDev.Core.Data.Extensions;
using AzDev.Core.DependencyInjection;
using AzDev.Core.DependencyInjection.Attributes;
using AzDev.Core.HealthCheck.Extensions;
using AzDev.Core.Logging.Extensions;
using AzDev.Core.Mapper.AutoMapper;
using AzDev.Core.Validation.FluentValidation.Extensions;
using AzDev.Core.Validation.FluentValidation.Filters;
using AzDev.Core.Versioning.Extensions;
using AzDev.Core.Web.Controller;
using AzDev.Core.Web.Extensions;
using AzDev.Core.Web.Middleware;
using BudgetApi.Context;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Config;

[Configuration]
public class WebAppBuilderConfig
{
    public void Configure(WebApplicationBuilder builder)
    {
        Assembly[] assemblies = [typeof(Program).Assembly];
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
        builder
            // .AddAuthorization()
            // .AddJwtTokenAuthentication()
            .AddExceptionHandler()
            .AddSwagger()
            .AddDapper()
            .AddHealthChecks()
            .AddLogging()
            .AddAutoMapper(assemblies)
            .AddFluentValidation(assemblies)
            .AddApiVersioning();
        
        builder.Services.AddControllers(configure =>
        {
            configure.Conventions.Add(
                new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
            
            configure.Filters.Add<FluentValidationFilter>();
            configure.Filters.Add<ResponseWrapperFilter>();
            configure.AddCustomFilters(assemblies);
        });

        builder.Services.AddDbContext<BudgetDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetDbConnectionString());
        });
    }
} 