using System;
using System.ComponentModel.DataAnnotations;

namespace LoginRegistration.Models
{
    public class LogUser
    {
        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string LogEmail {get; set;}

        [Required(ErrorMessage = "Password Required")]
        [MinLength(8, ErrorMessage = "Must be at least 8 characters")]
        public string LogPassword {get; set;}
    }
}