using RemindMe.Models;

namespace RemindMe.Data
{
    public interface IReminderRepository
    {
        Reminder GetReminderById(long id);
    }
}