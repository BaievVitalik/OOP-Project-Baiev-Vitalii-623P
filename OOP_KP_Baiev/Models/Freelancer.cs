namespace OOP_KP_Baiev.Models
{
    public class Freelancer : User
    {
        public List<Guid> AppliedProjectIds { get; set; } = new();
        public override string UserType() => "Freelancer";
    }
}