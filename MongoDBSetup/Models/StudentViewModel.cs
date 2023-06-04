namespace MongoDBSetup.Models
{
    public class StudentViewModel : Student
    {
        public Gender[] Genders { get; set; } = Array.Empty<Gender>();
        public new Gender Gender => Genders.FirstOrDefault() ?? new();
    }
}
