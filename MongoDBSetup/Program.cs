using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDBSetup.Models;
using MongoDBSetup.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection(nameof(MongoDBSettings)));
builder.Services.AddSingleton<IMongoDBSettings>(config => config.GetRequiredService<IOptions<MongoDBSettings>>().Value);
builder.Services.AddScoped<IMongoClient>(config => new MongoClient(builder.Configuration.GetValue<string>("MongoDBSettings:ConnectionString")));
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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
