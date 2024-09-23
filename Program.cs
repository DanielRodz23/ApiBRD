using ApiBRD.Models.Entities;
using ApiBRD.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string cadena = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception();

builder.Services.AddDbContext<LabsystePwaBrdContext>(options => options.UseMySql(cadena, ServerVersion.AutoDetect(cadena)));
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHub<CategoriaHub>("/hub");
app.MapControllers();

app.Run();
