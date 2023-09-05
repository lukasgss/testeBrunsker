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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(connectionString));
});

builder.Services.ConfigurarIdentity();

builder.Services.ConfigurarJwt(builder.Configuration);

builder.Services.
    ConfigurarApplication(builder.Configuration)
    .ConfigurarInfrastrutura();

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