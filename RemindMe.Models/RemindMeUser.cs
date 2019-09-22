using System.ComponentModel.DataAnnotations;
using System;

namespace RemindMe.Models
{
    public class RemindMeUser
    {
        [Key]
        public long UserId {get; set;}
        public string Username {get; set;}
        public DateTime CreatedDate {get; set;}
        public DateTime? MostRecentLogin {get; set;}

    }
}