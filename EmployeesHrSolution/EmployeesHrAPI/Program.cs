using AutoMapper;
using EmployeesHrApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace EmployeesHrAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var employeesConnectionString = builder.Configuration.GetConnectionString("employees") ?? throw new Exception("Need a Connection String");

            builder.Services.AddDbContext<EmployeeDataContext>(options =>
            {
                options.UseSqlServer(employeesConnectionString);
            });

            var mapperConfig = new MapperConfiguration(opt =>
            {
                opt.AddProfile<EmployeesHrApi.AutomapperProfiles.Employees>();
                opt.AddProfile<EmployeesHrApi.AutomapperProfiles.HiringRequestProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            builder.Services.AddSingleton<IMapper>(mapper);
            builder.Services.AddSingleton<MapperConfiguration>(mapperConfig);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); //code that lets you get the documentation.
                app.UseSwaggerUI(); // the middleware that provides the UI for viewing that documentation.
            }

            app.UseAuthorization();


            app.MapControllers(); //When doing controler based apis, this is where it creates the 'lookup'/route table.

            app.Run(); // this is when our API is up and runnig. And it "blocks" here.
        }
    }
}