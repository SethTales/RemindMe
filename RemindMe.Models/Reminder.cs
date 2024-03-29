using System;

namespace RemindMe.Models
{
    public class Reminder
    {
        public long ReminderId {get; set;}
        public long UserIdFk {get; set;}
        public string Message {get; set;}
        public string CronExpression {get; set;}
        public string RecipientPhoneNumber {get; set;}
        public DateTime CreatedDate {get; set;}
        public DateTime LastModifiedDate {get; set;}
        public DateTime LastRunDate {get; set;}
    }
}