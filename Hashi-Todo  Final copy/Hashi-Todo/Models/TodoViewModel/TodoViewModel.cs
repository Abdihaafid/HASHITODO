using System.Collections.Generic;
using Hashi_Todo.Models;

namespace Todo.Models.ViewModels
{
    public class TodoViewModel
    {
        public List<TodoItem> TodoList { get; set; }
        public TodoItem Todo { get; set; }

        // Include a User property for associating the user with the Todo items
        public User User { get; set; }
    }
}