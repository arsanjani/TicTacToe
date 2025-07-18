using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using TicTacToe.Server.Services;
using TicTacToe.Server.Hubs;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Server.Data;
using TicTacToe.Server.Mappings;

namespace TicTacToe.Server.app_Start
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers
            services.AddControllers();

            // Add OpenAPI/Swagger
            services.AddOpenApi();

            // Add Entity Framework
            services.AddDbContext<TicTacToeDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Add AutoMapper
            services.AddAutoMapper(typeof(GameMappingProfile));

            // Add CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    if (Environment.IsDevelopment())
                    {
                        builder.WithOrigins("http://localhost:55577", "https://localhost:55577", "http://localhost:5272", "https://localhost:5272")
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    }
                    else
                    {
                        builder.WithOrigins("http://localhost:55577", "https://localhost:55577")
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    }
                });
            });

            // Add SignalR
            services.AddSignalR();

            // Add game services (kept as Singleton for in-memory state, uses IServiceScopeFactory for DB operations)
            services.AddSingleton<IGameService, GameService>();

            // Configure Kestrel options for development
            if (Environment.IsDevelopment())
            {
                services.Configure<KestrelServerOptions>(options =>
                {
                    options.AllowSynchronousIO = true;
                });
            }

            // Add any additional services here
            // services.AddScoped<IMyService, MyService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Use default files and static assets
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/gameHub");
                endpoints.MapFallbackToFile("/index.html");
                endpoints.MapStaticAssets();
                
                // Add OpenAPI endpoints for development
                if (env.IsDevelopment())
                {
                    endpoints.MapOpenApi();
                }
            });
        }
    }
} 