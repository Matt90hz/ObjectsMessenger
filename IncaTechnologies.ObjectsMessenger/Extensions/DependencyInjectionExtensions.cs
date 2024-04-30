using IncaTechnologies.ObjectsMessenger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace IncaTechnologies.ObjectsMessenger.Extensions
{
    /// <summary>
    /// Utility method for DI
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Add as singleton the <see cref="MessengerHub"/> and all the <see cref="Messenger"/> implementations contained in the given <paramref name="assembly"/>. Also registers all <see cref="Messenger"/> into the <see cref="MessengerHub"/>.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="assembly">Where to look for <see cref="Messenger"/> to register.</param>
        /// <param name="messengerHub">It is possible to specify a <see cref="MessengerHub"/> different form <see cref="MessengerHub.Default"/>. Mainly for test purposes.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterMessengers(this IServiceCollection services, Assembly assembly, MessengerHub? messengerHub = null)
        {
            var messengerTypes = assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Messenger)))
                .ToArray();

            return services
                .AddSingletonMessengers(messengerTypes)
                .AddSingletonMessengerHub(messengerTypes, messengerHub ?? MessengerHub.Default);
        }

        static IServiceCollection AddSingletonMessengers(this IServiceCollection services, IEnumerable<Type> messengerTypes)
        {
            foreach (var type in messengerTypes)
            {
                services.AddSingleton(type);
            }

            return services;
        }

        static IServiceCollection AddSingletonMessengerHub(this IServiceCollection services, IEnumerable<Type> messengerTypes,  MessengerHub messengerHub)
        {
            Func<IServiceProvider, MessengerHub>? existingImplementationFactory = services
                .FirstOrDefault(serviceDescriptior => serviceDescriptior.ServiceType == typeof(MessengerHub))?
                .ImplementationFactory as Func<IServiceProvider, MessengerHub>;

            Func<IServiceProvider, MessengerHub> implementationFactory = serviceProvider =>
            {
                foreach (var messengerType in messengerTypes)
                {
                    messengerHub.RegisterMessenger((Messenger)serviceProvider.GetService(messengerType));
                }

                return messengerHub;
            };

            var combinedImplementationFactory = (Func<IServiceProvider, MessengerHub>)Delegate.Combine(existingImplementationFactory, implementationFactory);

            var serviceDescriptor = new ServiceDescriptor(typeof(MessengerHub), combinedImplementationFactory, ServiceLifetime.Singleton);

            services.Replace(serviceDescriptor);

            return services;
        }
    }
}
