using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using ZSocialMedia.Infrastructure.Cache;
using ZSocialMedia.Infrastructure.Database;

namespace ZSocialMedia.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddHealthChecks();

        builder.Services.Configure<DatabaseOptions>(options =>
                 builder.Configuration.GetSection(DatabaseOptions.SectionName).Bind(options));
        builder.Services.AddDatabaseServices(builder.Environment);

        builder.Services.Configure<RedisConfiguration>(
           builder.Configuration.GetSection(RedisConfiguration.SectionName));
        builder.Services.Configure<CacheConfiguration>(
            builder.Configuration.GetSection(CacheConfiguration.SectionName));
        builder.Services.AddRedisDistributedCache();


        var redisConfig = builder.Configuration.GetSection(RedisConfiguration.SectionName)
            .Get<RedisConfiguration>() ?? new RedisConfiguration();
        var databaseOptions = builder.Configuration.GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();
        builder.Services.AddHealthChecks()
            .AddRedis(redisConfig.ConnectionString, name: "Redis", tags: ["redis"])
            .AddNpgSql(databaseOptions.ConnectionString, name: "PostgreSQL", tags: ["db"]);


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        data = entry.Value.Data
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });

        app.MapControllers();

        app.Run();
    }
}