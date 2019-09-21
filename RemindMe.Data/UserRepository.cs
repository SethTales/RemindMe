using System.Threading.Tasks;
using RemindMe.Models;
using System;

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
    }
}