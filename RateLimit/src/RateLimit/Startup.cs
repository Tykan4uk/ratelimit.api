using System.Collections.Generic;
using System.Text.Json.Serialization;
using RateLimitApi.Configuration;
using RateLimitApi.Data;
using RateLimitApi.Data.Cache;
using RateLimitApi.Services;
using RateLimitApi.Services.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace RateLimitApi
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            AppConfiguration = builder.Build();
        }

        public IConfiguration AppConfiguration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                { Title = "RateLimitApi", Version = "v1" });
            });

            services.AddTransient<IRedisCacheConnectionService, RedisCacheConnectionService>();
            services.AddTransient<ICacheService<RateLimitCacheEntity>, CacheService<RateLimitCacheEntity>>();
            services.AddTransient<IJsonSerializer, JsonSerializer>();

            services.Configure<Config>(AppConfiguration);

            services.AddTransient<IRateLimitService, RateLimitService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint(
                        "/swagger/v1/swagger.json",
                        "RateLimitApi v1"));
            }

            app.UseRouting();
            app.UseEndpoints(builder => builder.MapDefaultControllerRoute());
        }
    }
}