using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddValidationDI(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddValidatorsFromAssemblyContaining<IApplicationMarker>();

            return serviceCollection;
        }
    }

    //Used to register all validators from application assembly
    interface IApplicationMarker { }
}
