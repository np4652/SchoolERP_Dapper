using Infrastructure.Interface;
using Microsoft.AspNetCore.DataProtection;
using Service;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
try
{

    WebAPI.Helpers.ServiceCollectionExtension.RegisterService(builder.Services, builder.Configuration);
    builder.Services.AddDataProtection().SetApplicationName($"{builder.Environment.EnvironmentName}")
        .PersistKeysToFileSystem(new DirectoryInfo($@"{builder.Environment.ContentRootPath}\keys"));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddScoped<IFeeGroupService,FeeGroupService>();
    builder.Services.AddScoped<IFeeVaribaleService, FeeVariableService>();
    builder.Services.AddScoped<IClassMasterService, ClassMasterService>();
    builder.Services.AddScoped<ISessionMasterService, SessionMasterService>();
    builder.Services.AddScoped<IStudentService, StudentService>();
    
    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseStaticFiles();
    app.UseCors(WebAPI.Helpers.ServiceCollectionExtension.corsPolicy);
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // Authentication & Authorization

    //app.UseAuthentication();

    //app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    // NLog: catch setup errors
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
}