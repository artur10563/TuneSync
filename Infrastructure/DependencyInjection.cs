using Application.Repositories;
using Application.Repositories.Shared;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
	public static class DependencyContainer
	{
		public static IServiceCollection DIFromContainer(this IServiceCollection serviceCollection, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("Default")
				?? throw new InvalidOperationException("Connection string 'Default' not found.");

			serviceCollection.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(connectionString));


			serviceCollection
				.AddScoped<IUnitOfWork, UnitOfWork>()
				.AddScoped<ISongRepository, SongRepository>()
				.AddScoped<IYoutubeService, YoutubeService>()
				.AddScoped<IFirebaseStorageService, FirebaseStorageService>();


			return serviceCollection;
		}
	}
}
