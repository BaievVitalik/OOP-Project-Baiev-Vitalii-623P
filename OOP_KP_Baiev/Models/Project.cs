using OOP_KP_Baiev.Interfaces; 

namespace OOP_KP_Baiev.Models
{
    public class Project : IIdentifiable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CustomerId { get; set; } 
        public Guid? FreelancerId { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string OwnerLogin { get; set; }
        public decimal Price { get; set; }
        public List<Guid> RespondedFreelancers { get; set; } = new List<Guid>();
    }
}   