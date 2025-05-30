using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OOP_KP_Baiev.Services
{
    public static class AccountManager
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string ResourcesPath = Path.Combine(BaseDirectory, "Resources");
        private static readonly string UsersFilePath = Path.Combine(ResourcesPath, "users.json");
        public static List<User> Users { get; private set; } = new();

        public static void LoadData()
        {
            if (File.Exists(UsersFilePath))
            {
                string json = File.ReadAllText(UsersFilePath);
                try
                {
                    var root = JsonNode.Parse(json)?.AsArray();
                    if (root != null)
                    {
                        bool updated = false;
                        foreach (var node in root)
                        {
                            if (node is JsonObject userObj && !userObj.ContainsKey("UserType"))
                            {
                                if (userObj.ContainsKey("Portfolio"))
                                    userObj["UserType"] = "Freelancer";
                                else if (userObj.ContainsKey("Permissions"))
                                    userObj["UserType"] = "Admin";
                                else
                                    userObj["UserType"] = "Customer";
                                updated = true;
                            }
                        }

                        if (updated)
                        {
                            var updatedJson = root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
                            File.WriteAllText(UsersFilePath, updatedJson);
                        }

                        var options = new JsonSerializerOptions
                        {
                            Converters = { new UserJsonConverter() },
                            PropertyNameCaseInsensitive = true
                        };

                        Users = root
                            .Select(node => JsonSerializer.Deserialize<User>(node.ToJsonString(), options))
                            .Where(user => user != null)
                            .ToList();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при обробці users.json: {ex.Message}");
                }
            }
        }
        public static void UpdateUser(User updatedUser)
        {
            var user = Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user != null)
            {
                int index = Users.IndexOf(user);
                Users[index] = updatedUser;
            }
        }

        public static void SaveData()
        {
            try
            {
                Directory.CreateDirectory(ResourcesPath);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new UserJsonConverter() }
                };

                File.WriteAllText(UsersFilePath, JsonSerializer.Serialize(Users, options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні даних: {ex.Message}");
            }
        }

        public static bool AddUser(User user)
        {
            if (Users.Any(u => u.Login == user.Login || u.Email == user.Email))
                return false;

            if (string.IsNullOrWhiteSpace(user.AvatarPath))
            {
                user.AvatarPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Avatars", "default_avatar.png");
            }

            Users.Add(user);
            SaveData();
            return true;
        }


        public static void RemoveUser(Guid userId)
        {
            Users.RemoveAll(u => u.Id == userId);
            SaveData();
        }
    }
}