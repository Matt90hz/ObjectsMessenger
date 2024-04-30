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
using System.Reflection.Emit;

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

    [Fact]
    public void MessengerFromDifferentAssemblies_ShouldBeAllRegistered()
    {
        //arrange
        var assemblyName1 = new AssemblyName("assembly1");
        var assembly1 = AssemblyBuilder.DefineDynamicAssembly(assemblyName1, AssemblyBuilderAccess.Run);
        var module1 = assembly1.DefineDynamicModule(assemblyName1.Name!);
        var type1 = module1!
            .DefineType("Messenger1", TypeAttributes.Public | TypeAttributes.Abstract, typeof(Messenger<object, object, object>))
            .CreateType();
        var type2 = module1!
            .DefineType("Publisher1", TypeAttributes.Public | TypeAttributes.Abstract, typeof(Messenger<object, object>))
            .CreateType();

        var assemblyName2 = new AssemblyName("assembly2");
        var assembly2 = AssemblyBuilder.DefineDynamicAssembly(assemblyName2, AssemblyBuilderAccess.Run);
        var module2 = assembly2.DefineDynamicModule(assemblyName2.Name!);
        var type3 = module2!
            .DefineType("Messenger3", TypeAttributes.Public | TypeAttributes.Abstract, typeof(Messenger<object, object, object>))
            .CreateType();
        var type4 = module2!
            .DefineType("Publisher4", TypeAttributes.Public | TypeAttributes.Abstract, typeof(Messenger<object, object>))
            .CreateType();

        var messengerHub = (MessengerHub)Activator.CreateInstance(typeof(MessengerHub), true)!;

        IServiceCollection services = new ServiceCollection();

        //act
        services.RegisterMessengers(assembly1, messengerHub);
        services.RegisterMessengers(assembly2, messengerHub);

        //assert
        services
            .Select(serviceDescriptor => serviceDescriptor.ServiceType)
            .Should()
            .Contain(type1)
            .And.Contain(type2)
            .And.Contain(type3)
            .And.Contain(type4);
    }

    [Fact]
    public void MessengerFromDifferentAssemblies_ShouldBeAllRegisteredInMessengerHubAndTriggerEvents()
    {
        //arrange
        var assemblyName1 = new AssemblyName("assembly1");
        var assembly1 = AssemblyBuilder.DefineDynamicAssembly(assemblyName1, AssemblyBuilderAccess.Run);
        var module1 = assembly1.DefineDynamicModule(assemblyName1.Name!);
        var typeBuilder1 = module1!.DefineType("Messenger", TypeAttributes.Public, typeof(Messenger<object, object, object>));

        var isMessagePreseverdGetMethod = typeBuilder1.DefineMethod("get_IsMessagePreserved", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(bool), Type.EmptyTypes);
        var isMessagePreservedGetILGen = isMessagePreseverdGetMethod.GetILGenerator();
        isMessagePreservedGetILGen.Emit(OpCodes.Ldc_I4_0);
        isMessagePreservedGetILGen.Emit(OpCodes.Ret);

        var receiveMessageMethod = typeBuilder1.DefineMethod("ReceiveMessage", MethodAttributes.Family | MethodAttributes.Virtual, null, [typeof(object), typeof(object)]);
        var receiveMessageILGen = receiveMessageMethod.GetILGenerator();
        receiveMessageILGen.Emit(OpCodes.Ret);

        var sendMessageMethod = typeBuilder1.DefineMethod("SendMessage", MethodAttributes.Family | MethodAttributes.Virtual, typeof(object), [typeof(object)]);
        var sendMessageILGen = sendMessageMethod.GetILGenerator();
        sendMessageILGen.Emit(OpCodes.Ldnull);
        sendMessageILGen.Emit(OpCodes.Ret);

        var type1 = typeBuilder1.CreateType();

        var assemblyName2 = new AssemblyName("assembly2");
        var assembly2 = AssemblyBuilder.DefineDynamicAssembly(assemblyName2, AssemblyBuilderAccess.Run);
        var module2 = assembly2.DefineDynamicModule(assemblyName2.Name!);
        var typeBuilder2 = module2!.DefineType("Publisher", TypeAttributes.Public, typeof(Messenger<object, object>));

        var isDefaultGetMethod = typeBuilder2.DefineMethod("get_Default", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(object), Type.EmptyTypes);
        var isDefaultGetILGen = isDefaultGetMethod.GetILGenerator();
        isDefaultGetILGen.Emit(OpCodes.Ldc_I4_0);
        isDefaultGetILGen.Emit(OpCodes.Ret);

        var sendMessagePublishMethod = typeBuilder2.DefineMethod("SendMessage", MethodAttributes.Family | MethodAttributes.Virtual, typeof(object), [typeof(object)]);
        var sendMessagePublishILGen = sendMessagePublishMethod.GetILGenerator();
        sendMessagePublishILGen.Emit(OpCodes.Ldnull);
        sendMessagePublishILGen.Emit(OpCodes.Ret);

        var type2 = typeBuilder2.CreateType();

        var messengerHub = (MessengerHub)Activator.CreateInstance(typeof(MessengerHub), true)!;

        IServiceCollection services = new ServiceCollection();
     
        services.RegisterMessengers(assembly1, messengerHub);
        services.RegisterMessengers(assembly2, messengerHub);

        var serviceProvider = services.BuildServiceProvider();

        using var eventsMonitor = messengerHub.MessengersEvents.ToEvent().Monitor();

        //act
        _ = serviceProvider.GetRequiredService<MessengerHub>();//spawn the service to trigger the implementation factory
        var messenger = (Messenger<object, object, object>)serviceProvider.GetRequiredService(type1);
        var publisher = (Messenger<object, object>)serviceProvider.GetRequiredService(type2);

        messenger.Send(default!);
        publisher.Send(default!);

        //assert
        eventsMonitor
            .Should()
            .Raise(nameof(eventsMonitor.Subject.OnNext))
            .WithArgs<(Messenger Messenger, MessengerEvent Event)>(arg => arg.Messenger.GetType().Equals(type1));

        eventsMonitor
            .Should()
            .Raise(nameof(eventsMonitor.Subject.OnNext))
            .WithArgs<(Messenger Messenger, MessengerEvent Event)>(arg => arg.Messenger.GetType().Equals(type2));
    }
}
