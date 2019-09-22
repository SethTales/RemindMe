using System.Threading.Tasks;
using RemindMe.Models;
using System;
using System.Linq;

namespace RemindMe.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly RemindMeDatabaseContext _dbContext;
        public UserRepository(RemindMeDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddUserAsync(string username)
        {
            var remindMeUser = new RemindMeUser
            {
                Username = username,
                CreatedDate = DateTime.UtcNow
            };
            await _dbContext.RemindMeUsers.AddAsync(remindMeUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateMostRecentLoginTimeAsync(string username)
        {
            var user = LookupUserByUsername(username);
            user.MostRecentLogin = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public RemindMeUser LookupUserByUsername(string username)
        {
            return _dbContext.RemindMeUsers.FirstOrDefault(u => u.Username == username);
        }
    }
}