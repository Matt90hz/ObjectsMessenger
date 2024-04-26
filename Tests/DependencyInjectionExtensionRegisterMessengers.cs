using Microsoft.Extensions.DependencyInjection;
using IncaTechnologies.ObjectsMessenger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using FluentAssertions;
using Tests.Utililties;
using IncaTechnologies.ObjectsMessenger;
using System.Reactive.Linq;

namespace Tests;
public class DependencyInjectionExtensionRegisterMessengers
{
    [Fact]
    public void MessengersInTheAssembly_ShouldBeAddedToServiceCollection()
    {
        //arrange
        IServiceCollection services = new ServiceCollection();

        //act
        services.RegisterMessengers(Assembly.GetExecutingAssembly());

        //assert
        services
            .Select(serviceDescriptor => serviceDescriptor.ServiceType)
            .Should()
            .Contain(typeof(GuidMessenger));
    }

    [Fact]
    public void MessengerHub_ShouldBeAddedToServiceCollection()
    {
        //arrange
        IServiceCollection services = new ServiceCollection();

        //act
        services.RegisterMessengers(Assembly.GetExecutingAssembly());

        //assert
        services
            .Select(serviceDescriptor => serviceDescriptor.ServiceType)
            .Should()
            .Contain(typeof(MessengerHub));
    }

    [Fact]
    public void MessengerHub_ShouldReemitEventsAndErrors()
    {
        //arrange
        IServiceCollection services = new ServiceCollection()
            .AddTransient<Sender>()
            .AddTransient<Receiver>()
            .RegisterMessengers(Assembly.GetExecutingAssembly(), (MessengerHub)Activator.CreateInstance(typeof(MessengerHub), true)!);
     
        IServiceProvider serviceProvider = services.BuildServiceProvider();

        var messengerHub = serviceProvider.GetRequiredService<MessengerHub>();
        var messenger = serviceProvider.GetRequiredService<GuidMessenger>();
        var sender = serviceProvider.GetRequiredService<Sender>();
        var receiver = serviceProvider.GetRequiredService<Receiver>();

        using var monitorEvents = messengerHub.MessengersEvents.ToEvent().Monitor();
        using var monitorErrors = messengerHub.MessengersErrors.ToEvent().Monitor();

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);
        messenger.Receive(receiver);

        //assert
        monitorEvents.Should().Raise(nameof(monitorEvents.Subject.OnNext));
        monitorErrors.Should().Raise(nameof(monitorErrors.Subject.OnNext));
    }
}
