using System.Threading.Tasks;

namespace RemindMe.Data
{
    public interface IApplicationRepository
    {
        Task AddUserAsync(string username);
    }
}