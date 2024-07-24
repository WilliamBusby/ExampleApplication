using System;
using System.Threading.Tasks;

namespace ExampleApplication.Common.Helpers
{
    public class NotificationHelper : INotificationHelper
    {
        public async Task<bool> SendNotification(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public interface INotificationHelper
    {
        public Task<bool> SendNotification(object obj);
    }
}
