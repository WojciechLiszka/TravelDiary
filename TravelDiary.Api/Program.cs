using FluentValidation.AspNetCore;
using TravelDiary.Api.Middleware;
using TravelDiary.Application.Extensions;
using TravelDiary.Infrastructure.Extensions;
using TravelDiary.Infrastructure.Seeders;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["TravelDiaryStorage:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["TravelDiaryStorage:queue"], preferMsi: true);
});

var app = builder.Build();

var scope = app.Services.CreateScope();

var seeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();

await seeder.Seed();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }