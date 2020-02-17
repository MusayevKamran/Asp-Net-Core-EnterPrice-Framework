using System.ComponentModel.DataAnnotations;

namespace App.Infrastructure.CrossCutting.Identity.ViewModels.InputModels
{
    public class RegisterUserInputModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}