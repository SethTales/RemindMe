using Microsoft.EntityFrameworkCore;
using RemindMe.Models;

namespace RemindMe.Data
{
    public class RemindMeDatabaseContext : DbContext
    {
        public RemindMeDatabaseContext(DbContextOptions<RemindMeDatabaseContext> options) : base(options)
        {
            
        }

        public DbSet<RemindMeUser> RemindMeUsers {get; set;}
        public DbSet<Reminder> Reminders {get; set;}

        public void ExecuteDatabaseMigration()
        {
            Database.Migrate();
        }
    }
}
