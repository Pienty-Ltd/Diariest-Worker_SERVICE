using Pienty.Diariest.Core.Models.Database;

namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface IUserService
    {
        User GetUserWithId(long id);
        User GetUserWithEmail(string email);
        bool IsUserExistWithEmail(string email);
        bool UpdateUser(User user);
        bool AddUser(User user);
    }
}