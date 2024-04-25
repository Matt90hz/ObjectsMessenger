using IncaTechnologies.ObjectsMessenger;

namespace Tests.Utililties;

sealed class GuidMessenger : Messenger<Sender, Receiver, Guid>
{
    public bool IsMessagePreservedSwitch { get; set; } = false;

    public override bool IsMessagePreserved => IsMessagePreservedSwitch;

    protected override void ReceiveMessage(Receiver receiver, Guid message) => receiver.Value = message;

    protected override Guid SendMessage(Sender sender) => sender.Value;

}
