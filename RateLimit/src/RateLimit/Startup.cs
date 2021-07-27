using RateLimitApi.Configuration;
using RateLimitApi.Data.Cache;
using RateLimitApi.Services;
using RateLimitApi.Services.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;

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
            // accepts any access token issued by identity server
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://192.168.1.120:5000";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            // adds an authorization policy to make sure the token is for scope 'api1'
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "ratelimitapi.ratelimitapi");
                });
                options.AddPolicy("ApiScopeBff", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "ratelimitapi.ratelimitapibff");
                });
            });
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

            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict });
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(builder => builder.MapDefaultControllerRoute());
        }
    }
}