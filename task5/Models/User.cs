﻿namespace task5.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; }
    }
}
