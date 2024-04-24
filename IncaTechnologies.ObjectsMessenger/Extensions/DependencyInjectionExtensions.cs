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
        /// Registers all the <see cref="Messenger"/> contained in the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="assembly">Where to look for <see cref="Messenger"/> to register.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterMessengers(this IServiceCollection services, Assembly assembly)
        {
            var messengerTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Messenger)));

            foreach (var type in messengerTypes)
            {
                services.AddSingleton(type);
            }

            services.AddSingleton(sp => 
            { 
                foreach(var messenger in sp.GetServices<Messenger>())
                {
                    MessageHub.Default.RegisterMessenger(messenger);
                }

                return MessageHub.Default; 
            });

            return services;
        }
    }
}
