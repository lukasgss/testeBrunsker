using Api.Extensoes;
using Application;
using Application.Middlewares;
using Infrastructure;
using Infrastructure.Persistencia.DataContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.ConfigurarSwagger();

builder.Services.
    ConfigurarApplication(builder.Configuration)
    .ConfigurarInfrastrutura(builder.Configuration);

builder.Services.ConfigurarIdentity();

builder.Services.ConfigurarJwt(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<TratamentoErrosMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();