using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hashi_Todo.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Navigation property for TodoItems
        public List<TodoItem> TodoItems { get; set; }
    }
}
