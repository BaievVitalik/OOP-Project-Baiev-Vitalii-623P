namespace OOP_KP_Baiev.Services
{
    public static class AuthService
    {
        public static User? Login(string username, string password)
        {
            return AccountManager.Users.FirstOrDefault(u => u.Login == username && u.Password == password);
        }

        public static bool Register(User user, string password)
        {
            if (AccountManager.Users.Any(u => u.Login == user.Login || u.Email == user.Email))
                return false;

            user.Password = password;
            AccountManager.AddUser(user);
            return true;
        }
    }
}