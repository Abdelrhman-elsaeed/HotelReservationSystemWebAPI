using Application.AutoMapper.Profiles;
using Application.CQRS.RoomType.Command;
using Application.Helper;
using AutoMapper;
using Domain.Helper.Services;
using Domain.Repositories.Interfaces;
using HotelReservationSystem.API.Middlewares;
using Infrastructure.Data;
using Infrastructure.Helper;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace HotelReservationSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //====JWT Configuration===

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

            var keyBytes = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();
            
            //=========================

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // MediateR Configure
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AddRoomTypeCommand).Assembly);
            });

            // Register AutoMapper (Scanning the Application assembly for Profiles)
            builder.Services.AddAutoMapper(cfg =>{cfg.AddMaps(typeof(AutoMapperHelper).Assembly);});

            // Register DbContext with the connection string
            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //DI
            builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IRoomRepository,RoomRepository>();
            builder.Services.AddScoped<IFileHandlingService, FileHandlingService>();
            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
            builder.Services.AddScoped<GlobalErrorHandlerMiddleware>();
            builder.Services.AddScoped<TransactionMiddleware>();

            var app = builder.Build();

            app.UseMiddleware<GlobalErrorHandlerMiddleware>();
            app.UseMiddleware<TransactionMiddleware>();

            // AutoMapper (Use GetRequiredService instead of GetService to fail fast if it's missing)
            AutoMapperHelper.Mapper = app.Services.GetRequiredService<IMapper>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
