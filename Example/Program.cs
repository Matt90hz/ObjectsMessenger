using Example;

CurrentUserMessenger currentUserMessenger = new();
UsersViewModel usersViewModel = new(currentUserMessenger);
EditUserViewModel viewModelEditUser = new(currentUserMessenger);

usersViewModel.CurrentUser = usersViewModel.Users.First();

Console.WriteLine($"Current user: {usersViewModel.CurrentUser.Id}");
Console.WriteLine($"User to edit: {viewModelEditUser.User?.Id ?? Guid.Empty}");

usersViewModel.CurrentUser = usersViewModel.Users.Last();

Console.WriteLine($"Current user: {usersViewModel.CurrentUser.Id}");
Console.WriteLine($"User to edit: {viewModelEditUser.User?.Id ?? Guid.Empty}");
