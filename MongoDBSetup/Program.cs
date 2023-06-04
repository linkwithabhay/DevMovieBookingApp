using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MongoDBSetup.Configurations;
using MongoDBSetup.Data;
using MongoDBSetup.Kafka;
using MongoDBSetup.Models;
using MongoDBSetup.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection(nameof(MongoDBSettings)));
builder.Services.AddSingleton<IMongoDBSettings>(config => config.GetRequiredService<IOptions<MongoDBSettings>>().Value);

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));
builder.Services.AddSingleton<IJwtConfig>(config => config.GetRequiredService<IOptions<JwtConfig>>().Value);

builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection(nameof(KafkaConfig)));
builder.Services.AddSingleton<IKafkaConfig>(config => config.GetRequiredService<IOptions<KafkaConfig>>().Value);

builder.Services.AddSingleton<IMongoClient>(config => new MongoClient(builder.Configuration.GetValue<string>("MongoDBSettings:ConnectionString")));

builder.Services.AddSingleton<IDbContext, DbContext>();

builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

builder.Services
    .AddIdentityMongoDbProvider<AppUser, AppRole>(config =>
    {
        config.Password.RequireDigit = false;
        config.Password.RequireLowercase = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        config.Password.RequiredLength = 1;
        config.Password.RequiredUniqueChars = 0;
    },
    mongo =>
    {
        var baseUri = new Uri(builder.Configuration.GetValue<string>("MongoDBSettings:ConnectionString"));
        var dbUri = new Uri(baseUri, builder.Configuration.GetValue<string>("MongoDBSettings:DatabaseName"));
        mongo.ConnectionString = dbUri.AbsoluteUri;
    });

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    var secretBytes = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtConfig:Secret"));
    var key = new SymmetricSecurityKey(secretBytes);
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JwtConfig:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("JwtConfig:Audience"),
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo() { Title = "Dev-MovieBookingApp", Version = "1.0.0" });
    s.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
        Name = "Authorization",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n\r\nExample: \"safsfs$d.fdf76d#hg.fytbju76t7g\""
    });
    s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "BearerAuth",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new string[] {}
                }});
});



var app = builder.Build();

DatabaseSeedData.Seed(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dev-MovieBookingApp (v1.0.0)"));
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();