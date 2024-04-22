namespace Example;

public sealed class UsersViewModel(CurrentUserMessenger _currentUserMessenger)
{
    private User? _currentUser;

    public User? CurrentUser
    {
        get { return _currentUser; }
        set 
        { 
            _currentUser = value;
            _currentUserMessenger.Send(this);
        }
    }

    public IEnumerable<User> Users { get; } = [new(Guid.NewGuid()), new(Guid.NewGuid()), new(Guid.NewGuid()),];
}
