namespace OOP_KP_Baiev.Interfaces
{
    public interface IEditableProfile
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        DateTime? BirthDate { get; set; }
        string Country { get; set; }
        string City { get; set; }
        string Description { get; set; }
        string AvatarPath { get; set; }
    }
}