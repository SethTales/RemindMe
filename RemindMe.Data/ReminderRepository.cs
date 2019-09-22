using RemindMe.Models;
using System.Linq;
using System;
using System.Collections.Generic;

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

        public void UpdateReminderLastRunTime(long id)
        {
            var storedReminder = GetReminderById(id);
            storedReminder.LastRunDate = DateTime.UtcNow;
            _dbContext.SaveChanges();
        }

        public IEnumerable<Reminder> GetRemindersForUser(long userId)
        {
            return _dbContext.Reminders.Where(r => r.UserIdFk == userId);
        }
    }

    
}