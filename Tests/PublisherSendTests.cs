using FluentAssertions;
using System.Reflection;
using Tests.Utililties;

namespace Tests;

public sealed class PublisherSendTests
{
    [Fact]
    public void MessageField_ShouldGetMessageValue()
    {
        //arrange
        Sender sender = new();
        GuidPublisher publisher = new();
        FieldInfo messageField = publisher
            .GetType()
            .BaseType!
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(field => field.Name == "_message");

        //act
        publisher.Send(sender);

        //assert
        messageField
            .GetValue(publisher)
            .Should()
            .Be(sender.Value);

    }
}
