using System.Threading.Tasks;
using auth.Models.DTO;

namespace auth.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(LoginModel model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(RegistrationModel model);
        Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username);
        Task<Status> RegisterDoctorAsync(RegistrationDoctorModel model);
    }
}
