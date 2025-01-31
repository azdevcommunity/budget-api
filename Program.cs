using System.Reflection;
using AzDev.Core.DependencyInjection;

Assembly[] assemblies = [typeof(Program).Assembly];

var builder = WebApplication.CreateBuilder(args);

builder.AddAutoConfigurationAndServices(assemblies);

var app = builder.Build();
app.AddAutoConfiguration(assemblies);

app.Run();