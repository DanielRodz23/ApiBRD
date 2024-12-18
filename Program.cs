using ApiBRD.Helpers;
using ApiBRD.Models.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiBRD.Repositories;
using ApiBRD.Hubs;
using Microsoft.OpenApi.Models;

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
builder.Services.AddTransient<Repository<Menudeldia>>();
builder.Services.AddTransient<Repository<Producto>>();
builder.Services.AddTransient<Repository<Categoria>>();
builder.Services.AddTransient<Repository<Usuario>>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
    var issuer = builder.Configuration.GetSection("Jwt").GetValue<string>("Issuer");
    var audience = builder.Configuration.GetSection("Jwt").GetValue<string>("Audience");
    var secret = builder.Configuration.GetSection("Jwt").GetValue<string>("Secret");

    x.TokenValidationParameters = new()
    {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true
    };
}
);


builder.Services.AddTransient<JwtTokerGenerator>();

builder.Services.AddSwaggerGen(options =>
{
     // add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, Array.Empty<string>()}
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<CategoriaHub>("/hub");
app.MapControllers();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();
