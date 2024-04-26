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
public sealed class DependencyInjectionExtensionRegisterMessengers
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
            .Contain(typeof(GuidMessenger))
            .And
            .Contain(typeof(GuidPublisher));
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

public sealed class MessengerExtensionsSetAndSand
{
    [Fact]
    public void ValueOfThePrperty_ShouldBeSetted()
    {
        //arrange
        Guid field1 = Guid.Empty;
        Guid field2 = Guid.Empty;
        Guid newGuid = Guid.NewGuid();
        GuidMessenger messenger = new();
        GuidPublisher publisher = new();
        Sender sender = new();

        //act
        messenger.SetAndSend(sender, ref field1, newGuid);
        publisher.SetAndSend(sender, ref field2, newGuid);

        //assert
        field1.Should().Be(newGuid);
        field2.Should().Be(newGuid);
    }

    [Fact]
    public void ValueGetSet_ShouldBeAlsoSent()
    {
        //arrange
        Guid field = Guid.Empty;
        Guid newGuid = Guid.NewGuid();
        GuidMessenger messenger = new();
        GuidPublisher publisher = new();
        Sender sender = new();

        var messengerEventsMonitor = messenger.Events.ToEvent().Monitor();
        var publisherEventsMonitor = publisher.Events.ToEvent().Monitor();

        //act
        messenger.SetAndSend(sender, ref field, newGuid);
        publisher.SetAndSend(sender, ref field, newGuid);

        //assert
        messengerEventsMonitor
            .Should()
            .Raise(nameof(messengerEventsMonitor.Subject.OnNext))
            .WithArgs<MessengerEvent>(me => me == MessengerEvent.Sended);

        publisherEventsMonitor
            .Should()
            .Raise(nameof(publisherEventsMonitor.Subject.OnNext))
            .WithArgs<MessengerEvent>(me => me == MessengerEvent.Sended);
    }
}