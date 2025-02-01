using AzDev.Core.APIDocumentation.Swagger.Extensions;
using AzDev.Core.DependencyInjection.Attributes;
using AzDev.Core.HealthCheck.Extensions;
using AzDev.Core.Web.Extensions;

namespace BudgetApi.Config;

[WebApplicationConfiguration]
public class WebConfig
{
    public void Configure(WebApplication app)
    {
        app.UseAzDevExceptionHandler();
        app.UseSwaggerDocumentation();
        app.UseHttpsRedirection();
        app.UseCors("AllowAllOrigins");
        // app.UseAuthorization();
        // app.UseAuthentication();
        app.MapControllers();
        app.UseAzDevHealthChecks();
    }
}