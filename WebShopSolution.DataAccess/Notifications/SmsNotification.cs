using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Notifications;

public class SmsNotification : INotificationObserver<Product>
{
    public void Update(Product entity)
    {
        Console.WriteLine($"Sms Notification: New product added - {entity.Name}");
    }
}