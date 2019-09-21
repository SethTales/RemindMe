using RemindMe.Models;
using System.Linq;

namespace RemindMe.Data
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly RemindMeDatabaseContext _dbContext;
        public ReminderRepository(RemindMeDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Reminder GetReminderById(long id)
        {
            return _dbContext.Reminders.FirstOrDefault(r => r.ReminderId == id);
        }
    }

    
}