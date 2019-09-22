using Amazon.SimpleNotificationService;
using RemindMe.Models;
using Amazon.SimpleNotificationService.Model;

namespace RemindMeRunner.App.Services
{
    public interface ISenderService
    {
        void SendReminder(Reminder reminder);
    }
    public class SenderService : ISenderService 
    {
        private readonly IAmazonSimpleNotificationService _snsClient;
        public SenderService(IAmazonSimpleNotificationService snsClient)
        {
            _snsClient = snsClient;
        }

        public void SendReminder(Reminder reminder)
        {
            var request = new PublishRequest
            {
                PhoneNumber = reminder.RecipientPhoneNumber,
                Message = reminder.Message
            };
            _snsClient.PublishAsync(request).Wait();
        }
    }
}