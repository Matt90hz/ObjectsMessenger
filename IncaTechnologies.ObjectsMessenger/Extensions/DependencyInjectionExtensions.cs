using IncaTechnologies.ObjectsMessenger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var messengerTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Messenger)));

            foreach (var type in messengerTypes)
            {
                services.AddSingleton(type);
            }

            messengerHub ??= MessengerHub.Default;

            services.AddSingleton(sp => 
            { 
                foreach(var messengerType in messengerTypes)
                {
                    messengerHub.RegisterMessenger((Messenger)sp.GetService(messengerType));
                }

                return messengerHub; 
            });

            return services;
        }
    }
}
