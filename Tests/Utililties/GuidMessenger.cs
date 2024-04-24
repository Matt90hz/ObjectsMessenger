using IncaTechnologies.ObjectsMessenger;

namespace Tests.Utililties;

sealed class GuidMessenger : Messenger<Sender, Receiver, Guid>
{
    public override bool IsMessagePreserved => false;

    protected override void ReceiveMessage(Receiver receiver, Guid message) => receiver.Value = message;

    protected override Guid SendMessage(Sender sender) => sender.Value;
}
