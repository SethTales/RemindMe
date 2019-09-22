using System.Threading.Tasks;
using RemindMe.Models;

namespace RemindMe.Data
{
    public interface IUserRepository
    {
        Task AddUserAsync(string username);
        Task UpdateMostRecentLoginTimeAsync(string username);
        RemindMeUser LookupUserByUsername(string username);
    }
}