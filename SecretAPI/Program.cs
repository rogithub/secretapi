using System;
using System.IO;
using Ro.SQLite.Data;
using SecretAPI.Repos;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// IoC
var connectionString = builder.Configuration.GetSection("DefaultConnection").Value;
builder.Services.AddTransient<IDbAsync>((svc) => {
    var current = Directory.GetCurrentDirectory();    
    var path = Path.Combine(current, connectionString);    
    return new Database(string.Format("Data Source={0}; Version=3;", path));
});
builder.Services.AddScoped<ISecretsRepo, SecretsRepo>();


var app = builder.Build();

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
