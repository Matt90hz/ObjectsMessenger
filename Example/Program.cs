using Example;
using System.Reactive.Linq;

CurrentUserMessenger currentUserMessenger = new();
CurrentUserPublisher currentUserPublisher = new();
UsersViewModel usersViewModel = new(currentUserMessenger, currentUserPublisher);
EditUserViewModel viewModelEditUser = new(currentUserMessenger, currentUserPublisher);

usersViewModel.CurrentUser = usersViewModel.Users.First();

Console.WriteLine($"Current user: {usersViewModel.CurrentUser.Id}");
Console.WriteLine($"User to edit: {viewModelEditUser.User?.Id ?? Guid.Empty}");

usersViewModel.CurrentUser = usersViewModel.Users.Last();

Console.WriteLine($"Current user: {usersViewModel.CurrentUser.Id}");
Console.WriteLine($"User to edit: {viewModelEditUser.User?.Id ?? Guid.Empty}");