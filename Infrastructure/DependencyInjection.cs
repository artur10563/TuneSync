using System.Security.Claims;
using System.Text.Json;
using Application;
using Application.CQ.Songs.Command.CreateSong;
using Application.Repositories;
using Application.Repositories.Shared;
using Application.Services;
using Application.Services.Auth;
using BlockchainSQL.Interceptors;
using Domain.Entities;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Shared;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public sealed class FirebaseConfiguration
    {
        public string type { get; set; }
        public string project_id { get; set; }
        public string private_key_id { get; set; }
        public string private_key { get; set; }
        public string client_email { get; set; }
        public string client_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_x509_cert_url { get; set; }
        public string universe_domain { get; set; }
    }

    public static class DependencyContainer
    {
        public static IServiceCollection DIFromContainer(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddValidationDI(configuration);
            
            var jsonCreds = configuration.GetSection("FirebaseConfig").Get<FirebaseConfiguration>();
            var credsString = JsonSerializer.Serialize(jsonCreds, new JsonSerializerOptions { WriteIndented = true });
            
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(credsString),
            });
            serviceCollection.AddScoped<StorageClient>(provider =>
            {
                return StorageClient.Create(GoogleCredential.FromJson(credsString));
            });

            serviceCollection.AddDatabase(configuration);
            serviceCollection.AddRepositories();
            serviceCollection.AddServices();

            serviceCollection.AddJwtAuth(configuration);

            serviceCollection.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateSongCommand).Assembly));
            
            //Scoped
            serviceCollection.AddHttpClient<IJwtService, JwtService>((http) =>
            {
                http.BaseAddress = new Uri(configuration["Auth:TokenUrl"]);
            });


            return serviceCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<ISongRepository, SongRepository>()
                .AddScoped<IYoutubeService, YoutubeService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IPlaylistRepository, PlaylistRepository>()
                .AddScoped<IStorageService, FirebaseStorageService>()
                .AddScoped<ILinkEntityRepository<PlaylistSong>, LinkEntityRepository<PlaylistSong>>()
                .AddScoped<ILinkEntityRepository<UserSong>, LinkEntityRepository<UserSong>>()
                .AddScoped<ILinkEntityRepository<UserFavoriteAlbum>, LinkEntityRepository<UserFavoriteAlbum>>()
                .AddScoped<ILinkEntityRepository<UserFavoritePlaylist>, LinkEntityRepository<UserFavoritePlaylist>>()
                .AddScoped<IArtistRepository, ArtistRepository>()
                .AddScoped<IAlbumRepository, AlbumRepository>();
            
            return serviceCollection;
        }

        private static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IBackgroundJobService, BackgroundJobService>();
            serviceCollection.AddSingleton<IAuthService, AuthService>();
            serviceCollection.AddSingleton<ILoggerService, LoggerService>();
            serviceCollection.AddScoped<ISearchService, SearchService>();
            
            return serviceCollection;
        }

        private static IServiceCollection AddJwtAuth(this IServiceCollection serviceCollection,  IConfiguration configuration)
        {

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

                    
                    jwtOptions.Events = new JwtBearerEvents
                    { 
                        OnTokenValidated = async context =>
                        {
                            var serviceProvider = context.HttpContext.RequestServices;
                            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();

                            var externalId = context.Principal?.FindFirst("user_id")?.Value;
                            
                            if (string.IsNullOrEmpty(externalId))
                            {
                                context.Fail("Invalid token: User not found.");
                                return;
                            }

                            var user = await uow.UserRepository.GetByExternalIdAsync(externalId);
                            if (user == null)
                            {
                                context.Fail("User not found.");
                                return;
                            }
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                            var role = string.IsNullOrEmpty(user.Role) ? "User" : user.Role;
                            claimsIdentity?.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    };

                });
            serviceCollection.AddAuthorization();
            
            return serviceCollection;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default")
                                   ?? throw new InvalidOperationException("Connection string 'Default' not found.");
            
            serviceCollection.AddHangfire(config =>
                config.UsePostgreSqlStorage(options =>
                    options.UseNpgsqlConnection(connectionString)
                )
            );

            serviceCollection.AddHangfireServer(options=>
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

            serviceCollection
                .AddBlockchainSql(configuration)
                .AddQueryInterceptor(configuration);
            
            serviceCollection.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var interceptor = serviceProvider.GetRequiredService<DbQueryInterceptor>();

                options.UseNpgsql(connectionString)
                    .AddInterceptors(interceptor);
            });
            
            return serviceCollection;
        }
    }
}
