using SessionManagement.Models.DTO;

namespace SessionManagement.Interface
{
    public interface IUser
    {
        Task<bool> RegisterUser(AddUser user);
        Task<AuthenticationDTO> LoginUser(UserLogin user);
    }
}
