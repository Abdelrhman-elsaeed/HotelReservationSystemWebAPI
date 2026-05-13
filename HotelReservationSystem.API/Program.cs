using Application.AutoMapper.Profiles;
using Application.CQRS.Room.Command;
using AutoMapper;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace HotelReservationSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            var app = builder.Build();

            // AutoMapper (Use GetRequiredService instead of GetService to fail fast if it's missing)
            AutoMapperHelper.Mapper = app.Services.GetRequiredService<IMapper>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
