using System.ComponentModel.DataAnnotations;

namespace App.Infrastructure.CrossCutting.Identity.ViewModels.InputModels
{
    public class RefreshTokenInputModel
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}