using Application.DTOs.Automapper;
using Domain.Primitives;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddValidationDI(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            GlobalVariables.Initialize(configuration["FirestoreStorage:DefaultBucket"]);

            serviceCollection.AddValidatorsFromAssemblyContaining<IApplicationMarker>();

            serviceCollection.AddAutoMapper(typeof(SongMap));

            return serviceCollection;
        }
    }

    //Used to register all validators from application assembly
    interface IApplicationMarker { }
}
