using Application.DTOs.Automapper;
using Domain.Primitives;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.CQ.Playlists.Command.CreatePlaylistFromYoutube;

namespace Application
{
    public static partial class DependencyInjection
    {
        public static IServiceCollection AddValidationDI(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            GlobalVariables.Initialize(configuration["FirestoreStorage:DefaultBucket"]);

            serviceCollection.AddValidatorsFromAssemblyContaining<IApplicationMarker>();

            serviceCollection.AddAutoMapper(Assembly.GetAssembly(typeof(IApplicationMarker)));

            serviceCollection.AddBackgroundWorkers();
            return serviceCollection;
        }
    }

    //Used to register all validators from application assembly
    interface IApplicationMarker { }
}
