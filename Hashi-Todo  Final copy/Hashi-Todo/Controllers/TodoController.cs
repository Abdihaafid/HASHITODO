using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Hashi_Todo.Models;
using Todo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Hashi_Todo.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        //[AllowAnonymous]
        public IActionResult Index()
        {
            // Get the UserId of the currently authenticated user
            int userId = GetUserId();

            var todoListViewModel = GetAllTodos(userId);
            return View(todoListViewModel);
        }

        [HttpGet]
        public JsonResult PopulateForm(int id)
        {
            // Get the UserId of the currently authenticated user
            int userId = GetUserId();

            var todo = GetById(id, userId);
            return Json(todo);
        }

        internal TodoViewModel GetAllTodos(int userId)
        {
            List<TodoItem> todoList = new();

            using (SqliteConnection con = new SqliteConnection("Data Source=mydatabase.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"SELECT * FROM todo WHERE UserId = {userId}";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TodoItem todo = new TodoItem();
                            PopulateTodoItem(reader, todo);
                            todoList.Add(todo);
                        }
                    }
                }
            }

            return new TodoViewModel
            {
                TodoList = todoList
            };
        }

        internal TodoItem GetById(int id, int userId)
        {
            TodoItem todo = new();

            using (var connection = new SqliteConnection("Data Source=mydatabase.db"))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();
                    tableCmd.CommandText = $"SELECT * FROM todo WHERE Id = {id} AND UserId = {userId}";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PopulateTodoItem(reader, todo);
                        }
                        else
                        {
                            return todo;
                        }
                    }
                }
            }

            return todo;
        }

        public RedirectResult Insert(TodoItem todo)
        {
            // Get the UserId of the currently authenticated user
            int userId = GetUserId();

            using (SqliteConnection con = new SqliteConnection("Data Source=mydatabase.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "INSERT INTO todo (name, UserId) VALUES (@Name, @UserId)";
                    tableCmd.Parameters.AddWithValue("@Name", todo.Name);
                    tableCmd.Parameters.AddWithValue("@UserId", userId);
                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return Redirect("Index");
        }

        [HttpPost]
        public JsonResult Done(int id)
        {
            // Get the UserId of the currently authenticated user
            int userId = GetUserId();

            // Verify that the todo item belongs to the current user before deleting
            // Your validation logic here...

            using (SqliteConnection con = new SqliteConnection("Data Source=mydatabase.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"DELETE FROM todo WHERE Id = {id} AND UserId = {userId}";
                    tableCmd.ExecuteNonQuery();
                }
            }

            return Json(new { });
        }

        [HttpPost]
        public RedirectResult Update(TodoItem todo)
        {
            // Get the UserId of the currently authenticated user
            int userId = GetUserId();

            using (SqliteConnection con = new SqliteConnection("Data Source=mydatabase.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "UPDATE todo SET name = @Name WHERE Id = @Id AND UserId = @UserId";
                    tableCmd.Parameters.AddWithValue("@Name", todo.Name);
                    tableCmd.Parameters.AddWithValue("@Id", todo.Id);
                    tableCmd.Parameters.AddWithValue("@UserId", userId);
                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return Redirect("Index");
        }

        // Helper method to populate TodoItem properties from the reader
        private static void PopulateTodoItem(SqliteDataReader reader, TodoItem todo)
        {
            if (!reader.IsDBNull(0))
            {
                todo.Id = reader.GetInt32(0);
            }

            if (!reader.IsDBNull(1))
            {
                todo.Name = reader.GetString(1);
            }
            // Add more properties as needed
        }

        // Helper method to get the UserId of the currently authenticated user
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0; // Default value or handle accordingly
        }
    }
}
