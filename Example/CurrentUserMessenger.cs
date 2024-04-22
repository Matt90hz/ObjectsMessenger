using IncaTechnologies.ObjectsMessenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example;
public sealed class CurrentUserMessenger : Messenger<UsersViewModel, ViewModelEditUser, User>
{
    public override bool Preserve => false;

    protected override User GetMessage(UsersViewModel sender) => sender.CurrentUser ?? new(Guid.Empty);
}
