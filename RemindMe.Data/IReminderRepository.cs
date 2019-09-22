using RemindMe.Models;
using System.Collections.Generic;

namespace RemindMe.Data
{
    public interface IReminderRepository
    {
        Reminder GetReminderById(long id);
        void UpdateReminderLastRunTime(long id);
        IEnumerable<Reminder> GetRemindersForUser(long userId);
    }
}