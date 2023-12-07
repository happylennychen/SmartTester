using Microsoft.EntityFrameworkCore;
using SmartTesterLib;
using SmartTesterLib.DataAccess;

namespace SmartTesterAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSingleton<Automator>();
            builder.Services.AddDbContext<SmartTesterDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("SmartTesterDB")));
            builder.Services.AddScoped<IChamberRepository, ChamberRepository>();
            builder.Services.AddScoped<ITesterRepository, TesterRepository>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
        }
    }
}