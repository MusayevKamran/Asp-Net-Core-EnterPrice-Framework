namespace App.Infrastructure.CrossCutting.Identity.ViewModels.OutputModels
{
    public class AuthOutputModel<T> {
        public AuthOutputModel() { }

        public AuthOutputModel(T response) {
            AuthToken = response;
        }

        public T AuthToken { get; set; }
    }
}