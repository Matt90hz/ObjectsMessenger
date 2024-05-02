using IncaTechnologies.ObjectsMessenger;

namespace Example;

public sealed class UsersViewModel(CurrentUserMessenger _currentUserMessenger, CurrentUserPublisher _currentUserPublisher)
{
    private User? _currentUser;

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUserMessenger.SetAndSend(this, ref _currentUser, value);
            _currentUserPublisher.Send(this);
        }
    }

    public IEnumerable<User> Users { get; } = [new(Guid.NewGuid()), new(Guid.NewGuid()), new(Guid.NewGuid())];
}
