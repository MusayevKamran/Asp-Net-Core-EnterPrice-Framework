namespace App.Infrastructure.CrossCutting.Identity.ViewModels.OutputModels
{
    public class AuthSuccessOutputModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public double? ExpiresIn { get; set; }
    }
}