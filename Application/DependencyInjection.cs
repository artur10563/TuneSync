using Application.CQ.Users.Register;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddValidationDI(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();

            return serviceCollection;
        }
    }
}
