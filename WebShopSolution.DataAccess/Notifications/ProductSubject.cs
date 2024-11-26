using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Notifications
{
    // Subject som håller reda på observatörer och notifierar dem
    public class ProductSubject : ISubject<Product>
    {
        // Lista över registrerade observatörer
        private readonly List<INotificationObserver<Product>> _observers = new List<INotificationObserver<Product>>();

        public void Attach(INotificationObserver<Product> observer)
        {
            // Lägg till en observatör
            _observers.Add(observer);
        }

        public void Detach(INotificationObserver<Product> observer)
        {
            // Ta bort en observatör
            _observers.Remove(observer);
        }

        public void Notify(Product product)
        {
            // Notifiera alla observatörer om en ny produkt
            foreach (var observer in _observers)
            {
                observer.Update(product);
            }
        }
    }
}
