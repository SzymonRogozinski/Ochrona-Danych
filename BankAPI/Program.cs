using BankAPI.Context;
using BankAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedClass;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(swagger =>
{
	//This is to generate the Default UI of Swagger Documentation
	swagger.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "JWT Token Authentication API",
		Description = "ASP.NET Core 5.0 Web API"
	});
	// To Enable authorization using Swagger (JWT)
	swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
	});
	swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});

var cd = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddSingleton<PasswordChecker>();

string token = builder.Configuration.GetSection("AppSettings:Token").Value;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
	{
		// options.Authority = "https://localhost:5001";
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateAudience = false,
			ValidateIssuer = false,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token)),
			ValidateIssuerSigningKey = true,
		};
	});

builder.Services.AddCors(options =>
{
	options.AddPolicy("MyCorsePolicy",
		builder =>
		{
			builder.WithOrigins("https://localhost:7060")
				.AllowAnyHeader()
				.AllowAnyMethod();
		});
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyCorsePolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
