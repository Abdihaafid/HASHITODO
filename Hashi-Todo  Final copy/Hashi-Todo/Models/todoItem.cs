using System.ComponentModel.DataAnnotations.Schema;

namespace Hashi_Todo.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        // Foreign key property
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property for the User
        public User User { get; set; }
    }
}