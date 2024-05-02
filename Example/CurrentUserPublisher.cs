using IncaTechnologies.ObjectsMessenger;

namespace Example;

public sealed class CurrentUserPublisher : Messenger<UsersViewModel, User?>
{
    public override User? Default => default;

    protected override User? SendMessage(UsersViewModel sender) => sender.CurrentUser;
}