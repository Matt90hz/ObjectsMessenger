// See https://aka.ms/new-console-template for more information
using Example;

CurrentUserMessenger currentUserMessenger = new();
UsersViewModel usersViewModel = new(currentUserMessenger);
ViewModelEditUser viewModelEditUser = new(currentUserMessenger);

usersViewModel.CurrentUser = usersViewModel.Users.First();

Console.WriteLine($"Current user: {usersViewModel.CurrentUser.Id}");
Console.WriteLine($"User to edit: {viewModelEditUser.User.Id}");

usersViewModel.CurrentUser = usersViewModel.Users.Last();

Console.WriteLine($"Current user: {usersViewModel.CurrentUser.Id}");
Console.WriteLine($"User to edit: {viewModelEditUser.User.Id}");
