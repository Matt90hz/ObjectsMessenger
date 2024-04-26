using IncaTechnologies.ObjectsMessenger;

namespace Tests.Utililties;

sealed class GuidPublisher : Messenger<Sender, Guid>
{
    public override Guid Default => Guid.Empty;

    protected override Guid SendMessage(Sender sender) => sender.Value;
}
