﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using testing.Models;

namespace testing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly string connectionString;
        public UsersController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        }

        [HttpPost]
        public IActionResult CreateUser(UserDTO userDTO)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "insert into Person " + 
                        "(FName, LName, Gender, BDate, Username, Password) " +
                        "values (@FName, @LName, @Gender, @BDate, @Username, @Password)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FName", userDTO.FName);
                        command.Parameters.AddWithValue("@LName", userDTO.LName);
                        command.Parameters.AddWithValue("@Gender", userDTO.Gender);
                        command.Parameters.AddWithValue("@BDate", userDTO.BDate);
                        command.Parameters.AddWithValue("@Username", userDTO.Username);
                        command.Parameters.AddWithValue("@Password", userDTO.Password);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("User", "Sorry, we have an exception");
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult GetUsers() 
        {
            List<User> users = new List<User>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "select * from Person";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                User user = new User();

                                user.ID = reader.GetInt32(0);
                                user.FName = reader.GetString(1);
                                user.LName = reader.GetString(2);
                                user.Gender = reader.GetString(3);
                                user.BDate = reader.GetString(4);
                                user.Username = reader.GetString(5);
                                user.Password = reader.GetString(6);

                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("User", "Sorry, we have an exception");
                return BadRequest(ModelState);
            }

            return Ok(users);
        }
    }
}