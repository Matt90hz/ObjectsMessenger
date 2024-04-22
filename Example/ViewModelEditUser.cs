namespace Example;

public sealed class ViewModelEditUser(CurrentUserMessenger _currentUserMessenger)
{
    public User User => _currentUserMessenger.Receive(this);
}