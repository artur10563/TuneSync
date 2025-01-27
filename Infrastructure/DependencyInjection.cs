using System.Threading.RateLimiting;
using Application;
using Application.CQ.Songs.Command.CreateSong;
using Application.Repositories;
using Application.Repositories.Shared;
using Application.Services;
using Application.Services.Auth;
using Domain.Entities;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Shared;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyContainer
    {
        public static IServiceCollection DIFromContainer(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddValidationDI(configuration);

            var connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' not found.");

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("firebase.json")
            });

            serviceCollection.AddHangfire(config =>
                config.UsePostgreSqlStorage(options =>
                    options.UseNpgsqlConnection(connectionString)
                )
            );

            serviceCollection.AddHangfireServer(options=>
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

            serviceCollection.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
                {
                    jwtOptions.RequireHttpsMetadata = false; //ONLY FOR DEVELOPMENT!!!

                    jwtOptions.Authority = configuration["Auth:ValidIssuer"];
                    jwtOptions.Audience = configuration["Auth:Audience"];
                    jwtOptions.TokenValidationParameters.ValidIssuer = configuration["Auth:ValidIssuer"];

                });
            serviceCollection.AddAuthorization();

            serviceCollection.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateSongCommand).Assembly));

            serviceCollection
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<ISongRepository, SongRepository>()
                .AddScoped<IYoutubeService, YoutubeService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IPlaylistRepository, PlaylistRepository>()
                .AddScoped<IStorageService, FirebaseStorageService>()
                .AddScoped<ILinkEntityRepository<PlaylistSong>, LinkEntityRepository<PlaylistSong>>()
                .AddScoped<ILinkEntityRepository<UserSong>, LinkEntityRepository<UserSong>>()
                .AddScoped<IArtistRepository, ArtistRepository>()
                .AddScoped<IAlbumRepository, AlbumRepository>();


            serviceCollection.AddScoped<IBackgroundJobService, BackgroundJobService>();
            serviceCollection.AddSingleton<IAuthService, AuthService>();
            serviceCollection.AddSingleton<ILoggerService, LoggerService>();
            serviceCollection.AddScoped<ISearchService, SearchService>();

            //Scoped
            serviceCollection.AddHttpClient<IJwtService, JwtService>((http) =>
            {
                http.BaseAddress = new Uri(configuration["Auth:TokenUrl"]);
            });


            return serviceCollection;
        }

        public static IServiceCollection WithRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(option =>
            {
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                option.OnRejected = (context, token) =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerService>();
                    var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                    var path = context.HttpContext.Request.Path;

                    logger.Log(message: $"REQUEST LIMIT", LogLevel.Warning, context: new { ipAddress, path });

                    return ValueTask.CompletedTask;
                };
                
                option.AddPolicy("fixed", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions()
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromSeconds(10)
                        }
                    ));
            });
            
            return services;
        }
    }
}
