using System;
using System.ComponentModel.DataAnnotations;

namespace LoginRegistration
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage="First Name Required")]
        [MinLength(2, ErrorMessage = "First name must be 2 or more characters.")]
        public string First_Name { get; set; }

        [Required(ErrorMessage="Last Name Required")]
        [MinLength(2, ErrorMessage = "Last name must be 2 or more characters. I'm looking @ you Mr. Ng")]
        public string Last_Name { get; set; }

        [Required(ErrorMessage="Email Required")]
        [EmailAddress(ErrorMessage="Email Address is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage="Password Required")]
        [MinLength(8, ErrorMessage="Password must be 8 or more characters")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage="Passwords do not match!")]
        public string Confirm_Password { get; set; }
    }
}