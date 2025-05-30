using OOP_KP_Baiev.Interfaces;
using System.Text.Json.Serialization;

public abstract class User : IIdentifiable, IEditableProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Login { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Description { get; set; }
    public string? AvatarPath { get; set; }

    [JsonPropertyName("UserType")]
    public string Role => UserType();

    public abstract string UserType();
}
