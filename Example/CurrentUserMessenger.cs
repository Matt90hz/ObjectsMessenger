using IncaTechnologies.ObjectsMessenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example;
public sealed class CurrentUserMessenger : Messenger<UsersViewModel, EditUserViewModel, User?>
{
    public override bool IsMessagePreserved => false;

    protected override void ReceiveMessage(EditUserViewModel receiver, User? message) => receiver.User = message;

    protected override User? SendMessage(UsersViewModel sender) => sender.CurrentUser;

    override 
}
