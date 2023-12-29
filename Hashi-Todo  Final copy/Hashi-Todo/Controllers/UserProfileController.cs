using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Hashi_Todo.Models;
using Todo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Hashi_Todo.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        public IActionResult Index()
        {
            int userId = GetUserId();

            var userProfileViewModel = GetUserProfile(userId);
            return View(userProfileViewModel);
        }

        internal UserProfileViewModel GetUserProfile(int userId)
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=mydatabase.db"))
            {
                con.Open();

                var user = new User();
                using (var tableCmd = con.CreateCommand())
                {
                    tableCmd.CommandText = $"SELECT * FROM Users WHERE UserId = {userId}";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user.UserId = reader.GetInt32(0);
                            user.Username = reader.GetString(1);
                            // Add more properties if needed
                        }
                    }
                }

                var todoCount = GetTodoCount(userId);

                return new UserProfileViewModel
                {
                    User = user,
                    TodoCount = todoCount
                };
            }
        }

        internal int GetTodoCount(int userId)
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=mydatabase.db"))
            {
                con.Open();

                using (var tableCmd = con.CreateCommand())
                {
                    tableCmd.CommandText = $"SELECT COUNT(*) FROM todo WHERE UserId = {userId}";

                    return Convert.ToInt32(tableCmd.ExecuteScalar());
                }
            }
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
