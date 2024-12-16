using Domain.Primitives;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static partial class DependencyInjection
    {
        public static IServiceCollection AddValidationDI(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            GlobalVariables.Initialize(configuration["FirestoreStorage:DefaultBucket"]);

            serviceCollection.AddValidatorsFromAssemblyContaining<IApplicationMarker>();

            serviceCollection.AddBackgroundWorkers();
            return serviceCollection;
        }
    }

    //Used to register all validators from application assembly
    interface IApplicationMarker { }
}
