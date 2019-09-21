using System.Threading.Tasks;

namespace RemindMe.Data
{
    public interface IUserRepository
    {
        Task AddUserAsync(string username);
    }
}