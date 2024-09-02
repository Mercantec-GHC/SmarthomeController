using System;
using API.Context;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                );
            });
            IConfiguration Configuration = builder.Configuration;

            // Retrieve the connection string from configuration or environment variables
            string connectionString = Configuration.GetConnectionString("DefaultConnection")
                                      ?? Environment.GetEnvironmentVariable("CONNECTION_STRINGS_DEFAULT_CONNECTION");

            builder.Services.AddDbContext<SmartHomeContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Ensure the CORS middleware runs before the endpoint routing middleware
            app.UseCors(MyAllowSpecificOrigins);

            // Configure WebSocket middleware
            app.UseWebSockets();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // Map WebSocket endpoint
            app.Map("/ws/updates", (HttpContext context) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    return WebSocketHandler.HandleWebSocket(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    return Task.CompletedTask;
                }
            });

            app.Run();
        }
    }
}
