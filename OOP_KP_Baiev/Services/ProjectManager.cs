using OOP_KP_Baiev.Models;
using System.IO;
using System.Text.Json;

namespace OOP_KP_Baiev.Services
{
    public static class ProjectManager
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "projects.json"); // Уточнил путь
        public static List<Project> Projects { get; private set; } = new();

        static ProjectManager()
        {
            Load();
        }

        public static void Load()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                Projects = JsonSerializer.Deserialize<List<Project>>(json) ?? new();
            }
        }

        public static void RemoveProjectsByOwner(Guid userId)
        {
            Projects.RemoveAll(p => p.CustomerId == userId || p.FreelancerId == userId);
            Save();
        }

        public static void Save()
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (directory != null && !Directory.Exists(directory)) // Проверка на null для directory
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(Projects, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void Add(Project project)
        {
            Projects.Add(project);
            Save();
        }

        public static void Remove(string projectId) 
        {
            if (Guid.TryParse(projectId, out Guid parsedId))
            {
                Projects.RemoveAll(p => p.Id == parsedId);
                Save();
            }
        }
    }
}   