using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Notifications;

public class PushNotification : INotificationObserver<Product>
{
    public void Update(Product entity)
    {
        Console.WriteLine($"Push Notification: New product added - {entity.Name}");
    }
}