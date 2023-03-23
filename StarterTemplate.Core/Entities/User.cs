namespace StarterTemplate.Core.Entities
{
    /// <summary>
    /// Represents a Staff member
    /// </summary>
    public class User
    {
        public User(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Email { get; set; }
        public string Name { get; set; }
    }
}