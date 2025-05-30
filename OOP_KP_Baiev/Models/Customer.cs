namespace OOP_KP_Baiev.Models
{
    public class Customer : User
    {
        public List<Guid> CreatedProjectIds { get; set; } = new();
        public override string UserType() => "Customer";
    }
}