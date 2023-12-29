using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using BCrypt.Net;
using System.Data.SQLite;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hashi_Todo.Controllers
{
    public class UserController : Controller
    {
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(string username, string password)
        {
            // Check for username and password presence
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("Username", "Please enter a username.");
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Password", "Please enter a password.");
            }

            // If there are validation errors, return to the registration view
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Hash the password before storing it
            string hashedPassword = HashPassword(password);

            // Insert user into the Users table in SQLite
            string connectionString = "Data Source=mydatabase.db;";
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var tableCmd = connection.CreateCommand())
                {
                    tableCmd.CommandText = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                    tableCmd.Parameters.AddWithValue("@Username", username);
                    tableCmd.Parameters.AddWithValue("@Password", hashedPassword);
                    tableCmd.ExecuteNonQuery();
                }
            }

            // Redirect to login page
            return RedirectToAction("Login", "User");
        }



        private string HashPassword(string password)
        {
            
            // using BCrypt.Net
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            string connectionString = "Data Source=mydatabase.db;Version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var tableCmd = connection.CreateCommand())
                {
                    tableCmd.CommandText = "SELECT UserId, Password FROM Users WHERE Username = @Username";
                    tableCmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            string hashedPassword = reader["Password"].ToString();

                            // Check if the password is not null before verification
                            if (password != null && BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                            {
                                // Authentication successful, set session or cookie
                                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // Include UserId claim
                            // Add additional claims as needed
                        };

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var authProperties = new AuthenticationProperties
                                {
                                    // Customize any additional authentication properties
                                };

                                await HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    new ClaimsPrincipal(claimsIdentity),
                                    authProperties);

                                // Redirect to the main Todo list page
                                return RedirectToAction("Index", "Todo");
                            }
                        }


                        // Authentication failed, add a validation error
                        ModelState.AddModelError(string.Empty, "Invalid username or password");
                    }
                }
            }

            // Authentication failed, redirect back to login page
            return View();
        }



        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the login page
            return RedirectToAction("Login", "User");
        }



    }
}

