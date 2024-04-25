using FluentAssertions;
using IncaTechnologies.ObjectsMessenger;
using System.Reflection;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerSendTests
{
    [Fact]
    public void MessageField_ShouldGetMessageValue()
    {
        //arrange
        Sender sender = new();
        GuidMessenger messenger = new();
        FieldInfo messageField = messenger
            .GetType()
            .BaseType!
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(field => field.Name == "_message");

        //act
        messenger.Send(sender);

        //assert
        messageField
            .GetValue(messenger)
            .Should()
            .Be(sender.Value);

    }
}
