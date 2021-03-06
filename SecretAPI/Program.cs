using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ro.SQLite.Data;
using SecretAPI.Repos;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders =
		ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
	Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
	In = ParameterLocation.Header,
	Name = "Authorization",
	        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
	        options.TokenValidationParameters = new TokenValidationParameters
		{
		    ValidateIssuerSigningKey = true,
		    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
								.GetBytes(builder.Configuration.GetSection("JWT_TOKEN").Value)),
		    ValidateIssuer = false,
		    ValidateAudience = false
		};
    });

// IoC
var connectionString = builder.Configuration.GetSection("DefaultConnection").Value;
builder.Services.AddTransient<IDbAsync>((svc) => {
    return new Database(connectionString);
});
builder.Services.AddScoped<ISecretsRepo, SecretsRepo>();
builder.Services.AddScoped<IUsersRepo, UsersRepo>();


var app = builder.Build();

app.UseForwardedHeaders();

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
