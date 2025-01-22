﻿namespace FoodOutletRESTAPIDatabase.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  // Store hashed password
        public string Role { get; set; }  // e.g., Admin, User
    }
}
